using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Helpers;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Handles user authentication, profile management, and account-related operations.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext context, JwtHelper jwtHelper, EmailService emailService)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
        }

        // ===================== VIEWS =====================

        /// <summary>GET: /Account/Login</summary>
        public IActionResult Login() => View();

        /// <summary>GET: /Account/Register</summary>
        public IActionResult Register() => View();

        /// <summary>GET: /Account/Profile</summary>
        public IActionResult Profile() => View();

        /// <summary>GET: /Account/Orders</summary>
        public IActionResult Orders() => View();

        /// <summary>GET: /Account/ForgotPassword</summary>
        public IActionResult ForgotPassword() => View();

        /// <summary>GET: /Account/ResetPassword</summary>
        public IActionResult ResetPassword(string token, string otp)
        {
            ViewData["Token"] = token;
            ViewData["Otp"] = otp;
            return View();
        }

        // ===================== AUTHENTICATION =====================

        /// <summary>POST: /Account/Register – Creates a new user account.</summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new AuthResponseDto { Message = string.Join(" | ", errors) });
            }

            // Check for duplicate email
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest(new AuthResponseDto { Message = "Email already registered." });

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Address = request.Address,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Send welcome email (non-blocking)
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] Welcome email failed: {ex.Message}");
            }

            return Ok(new AuthResponseDto
            {
                Message = "Registration Successful!",
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            });
        }

        /// <summary>POST: /Account/Login – Authenticates a user and returns a JWT token.</summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new AuthResponseDto { Message = "Invalid email or password." });

            if (request.AdminLogin && !user.IsAdmin)
                return Unauthorized(new AuthResponseDto { Message = "Access denied. You do not have admin privileges." });

            var token = _jwtHelper.GenerateToken(user, request.RememberMe);

            // Store user details in session for server-side authentication
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            if (request.AdminLogin && user.IsAdmin)
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                HttpContext.Session.SetInt32("AdminUserId", user.UserId);
            }

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                IsSuperAdmin = user.IsSuperAdmin,
                Message = "Login Successful!"
            });
        }

        /// <summary>POST: /Account/Logout – Clears the user session.</summary>
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out" });
        }

        // ===================== PROFILE MANAGEMENT =====================

        /// <summary>GET: /Account/GetProfile – Returns the current user's profile data.</summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.CreatedAt,
                    user.AvatarUrl
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to view your profile." });
            }
        }

        /// <summary>POST: /Account/UpdateProfile – Updates the current user's profile.</summary>
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.Address = request.Address;

                // Update email with duplicate check
                if (user.Email != request.Email)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == request.Email && u.UserId != userId);
                    if (existingUser != null)
                        return BadRequest(new { message = "Email already in use by another account." });
                    user.Email = request.Email;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully!", fullName = user.FullName });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to update your profile." });
            }
        }

        /// <summary>POST: /Account/UploadAvatar – Updates the user's avatar image.</summary>
        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromBody] AvatarUploadDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                user.AvatarUrl = request.AvatarBase64;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Avatar updated successfully!", avatarUrl = request.AvatarBase64 });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to update your avatar." });
            }
        }

        /// <summary>POST: /Account/ChangePassword – Changes the user's password.</summary>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                    return BadRequest(new { message = "Current password is incorrect." });

                if (request.NewPassword != request.ConfirmPassword)
                    return BadRequest(new { message = "New password and confirmation do not match." });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                // Send password change alert (non-blocking)
                try
                {
                    await _emailService.SendPasswordChangedAlertAsync(user.Email, user.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Email Error] Password change alert failed: {ex.Message}");
                }

                return Ok(new { message = "Password changed successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to change your password." });
            }
        }

        /// <summary>DELETE: /Account/DeleteAccount – Permanently deletes the user's account.</summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users
                    .Include(u => u.Orders)
                    .Include(u => u.ShoppingCartItems)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (user.ShoppingCartItems.Any())
                    _context.ShoppingCarts.RemoveRange(user.ShoppingCartItems);

                if (user.Orders.Any())
                    _context.Orders.RemoveRange(user.Orders);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Account deleted successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to delete your account." });
            }
        }

        // ===================== ADDRESS BOOK =====================

        /// <summary>GET: /Account/GetAddresses – Returns all addresses for the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.IsDefault)
                    .ToListAsync();
                return Ok(addresses);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to view your addresses." });
            }
        }

        /// <summary>POST: /Account/AddAddress – Adds a new address for the current user.</summary>
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var existingAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();

                // If no addresses exist, make this the default
                if (!existingAddresses.Any())
                    request.IsDefault = true;

                // If this address is default, unset all other defaults
                if (request.IsDefault)
                {
                    foreach (var addr in existingAddresses)
                        addr.IsDefault = false;
                }

                var address = new Address
                {
                    UserId = userId,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    City = request.City,
                    State = request.State,
                    Pincode = request.Pincode,
                    PhoneNumber = request.PhoneNumber,
                    IsDefault = request.IsDefault,
                    Label = request.Label
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Address added successfully!", address });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to add an address." });
            }
        }

        /// <summary>PUT: /Account/UpdateAddress/{id} – Updates an existing address.</summary>
        [HttpPut]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);

                if (address == null)
                    return NotFound(new { message = "Address not found." });

                // If this address is becoming default, unset all other defaults
                if (request.IsDefault && !address.IsDefault)
                {
                    var otherAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId && a.AddressId != id)
                        .ToListAsync();
                    foreach (var addr in otherAddresses)
                        addr.IsDefault = false;
                }

                address.AddressLine1 = request.AddressLine1;
                address.AddressLine2 = request.AddressLine2;
                address.City = request.City;
                address.State = request.State;
                address.Pincode = request.Pincode;
                address.PhoneNumber = request.PhoneNumber;
                address.IsDefault = request.IsDefault;
                address.Label = request.Label;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Address updated successfully!", address });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to update your address." });
            }
        }

        /// <summary>DELETE: /Account/DeleteAddress/{id} – Deletes an address.</summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);

                if (address == null)
                    return NotFound(new { message = "Address not found." });

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                // If the deleted address was default, make another address default
                if (address.IsDefault)
                {
                    var newDefault = await _context.Addresses
                        .FirstOrDefaultAsync(a => a.UserId == userId);
                    if (newDefault != null)
                    {
                        newDefault.IsDefault = true;
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { message = "Address deleted successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to delete your address." });
            }
        }

        // ===================== HELPERS =====================

        /// <summary>
        /// Gets the current user's ID from the JWT token claims.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated.</exception>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }
    }
}