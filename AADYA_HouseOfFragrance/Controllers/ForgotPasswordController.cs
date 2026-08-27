using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Handles password reset functionality via OTP (One-Time Password) sent to the user's email.
    /// Flow: Request OTP → Verify OTP → Reset Password.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ForgotPasswordController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ===================== STEP 1: REQUEST OTP =====================

        /// <summary>
        /// POST: api/ForgotPassword/request
        /// Sends a 6-digit OTP to the user's email address for password reset.
        /// If the email doesn't exist, returns a generic message for security.
        /// Prevents multiple active reset requests for the same user.
        /// </summary>
        [HttpPost("request")]
        public async Task<IActionResult> RequestReset([FromBody] ForgotPasswordRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.EmailOrPhone);

            // Security: Return generic message even if email doesn't exist
            if (user == null)
            {
                return Ok(new { message = "If your account exists, you will receive an OTP." });
            }

            // Check for existing active reset request
            var existingRequest = await _context.PasswordResets
                .FirstOrDefaultAsync(r => r.UserId == user.UserId && !r.IsUsed && r.ExpiresAt > DateTime.UtcNow);

            if (existingRequest != null)
            {
                return BadRequest(new
                {
                    message = "A reset request is already active. Please check your email or wait for it to expire."
                });
            }

            // Create new reset record
            var reset = new PasswordReset
            {
                UserId = user.UserId,
                ResetMethod = "Email",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Otp = GenerateOtp()
            };

            // Send OTP email
            try
            {
                var otpEmailBody = _emailService.GetOtpEmailBody(reset.Otp, user.FullName);
                await _emailService.SendEmailAsync(user.Email, "Your Password Reset OTP - AADYA", otpEmailBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] Failed to send OTP email: {ex.Message}");
                // Continue – the reset record will still be saved
            }

            _context.PasswordResets.Add(reset);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "OTP has been sent to your email.",
                method = "Email"
            });
        }

        // ===================== STEP 2: VERIFY OTP =====================

        /// <summary>
        /// POST: api/ForgotPassword/verify
        /// Verifies the provided OTP. If valid, marks it as used and returns
        /// a reset token for the password reset step.
        /// </summary>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
        {
            var reset = await _context.PasswordResets
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Otp == request.Otp &&
                    !r.IsUsed &&
                    r.ExpiresAt > DateTime.UtcNow);

            if (reset == null)
            {
                return BadRequest(new { message = "Invalid or expired OTP. Please request a new one." });
            }

            // Mark OTP as used
            reset.IsUsed = true;
            await _context.SaveChangesAsync();

            // Generate a new reset token for the next step
            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            // Store token in session for validation in the final step
            HttpContext.Session.SetString("ResetToken", resetToken);
            HttpContext.Session.SetInt32("ResetUserId", reset.UserId);

            return Ok(new
            {
                message = "OTP verified successfully.",
                resetToken = resetToken,
                userId = reset.UserId
            });
        }

        // ===================== STEP 3: RESET PASSWORD =====================

        /// <summary>
        /// POST: api/ForgotPassword/reset
        /// Resets the user's password using the verified reset token.
        /// The token must match the one stored in session.
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var sessionToken = HttpContext.Session.GetString("ResetToken");
            var sessionUserId = HttpContext.Session.GetInt32("ResetUserId");

            // Validate session token
            if (string.IsNullOrEmpty(sessionToken) || sessionUserId == null)
            {
                return BadRequest(new { message = "Reset session expired. Please request a new reset." });
            }

            if (sessionToken != request.ResetToken)
            {
                return BadRequest(new { message = "Invalid reset session." });
            }

            var user = await _context.Users.FindAsync(sessionUserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            // Hash and save new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Clear session data
            HttpContext.Session.Remove("ResetToken");
            HttpContext.Session.Remove("ResetUserId");

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully!" });
        }

        // ===================== HELPERS =====================

        /// <summary>
        /// Generates a random 6-digit OTP.
        /// </summary>
        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}