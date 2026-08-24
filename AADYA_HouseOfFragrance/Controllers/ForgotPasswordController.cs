using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AADYA_HouseOfFragrance.Controllers
{
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

        // POST: api/ForgotPassword/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestReset([FromBody] ForgotPasswordRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.EmailOrPhone);

            if (user == null)
            {
                return Ok(new { message = "If your account exists, you will receive an OTP." });
            }

            // Check existing active request
            var existingRequest = await _context.PasswordResets
                .FirstOrDefaultAsync(r => r.UserId == user.UserId && !r.IsUsed && r.ExpiresAt > DateTime.Now);

            if (existingRequest != null)
            {
                return BadRequest(new { message = "A reset request is already active. Please check your email or wait for it to expire." });
            }

            var reset = new PasswordReset
            {
                UserId = user.UserId,
                ResetMethod = "Email",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10),
                IsUsed = false
            };

            reset.Otp = GenerateOtp();

            // Send only OTP email (no link)
            try
            {
                var otpEmailBody = _emailService.GetOtpEmailBody(reset.Otp, user.FullName);
                await _emailService.SendEmailAsync(user.Email, "Your Password Reset OTP - AADYA", otpEmailBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                // Continue – we'll still return OTP for testing
            }

            _context.PasswordResets.Add(reset);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "OTP has been sent to your email.",
                method = "Email",
            });
        }

        // POST: api/ForgotPassword/verify
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
        {
            var reset = await _context.PasswordResets
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Otp == request.Otp &&
                    !r.IsUsed &&
                    r.ExpiresAt > DateTime.Now);

            if (reset == null)
            {
                return BadRequest(new { message = "Invalid or expired OTP. Please request a new one." });
            }

            reset.IsUsed = true;
            await _context.SaveChangesAsync();

            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            HttpContext.Session.SetString("ResetToken", resetToken);
            HttpContext.Session.SetInt32("ResetUserId", reset.UserId);

            return Ok(new
            {
                message = "OTP verified successfully.",
                resetToken = resetToken,
                userId = reset.UserId
            });
        }

        // POST: api/ForgotPassword/reset
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var sessionToken = HttpContext.Session.GetString("ResetToken");
            var sessionUserId = HttpContext.Session.GetInt32("ResetUserId");

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

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            HttpContext.Session.Remove("ResetToken");
            HttpContext.Session.Remove("ResetUserId");

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully!" });
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}