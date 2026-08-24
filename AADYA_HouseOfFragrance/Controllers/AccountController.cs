using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Helpers;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AccountController(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            return View();
        }

        // GET: /Account/Orders
        public IActionResult Orders()
        {
            return View();
        }

        // POST: /Account/Register
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

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResponseDto { Message = "Email already registered." });
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                Address = request.Address,
                CreatedAt = DateTime.Now
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                Message = "Registration Successful!",
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            });
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new AuthResponseDto { Message = "Invalid email or password." });
            }

            if (request.AdminLogin && !user.IsAdmin)
            {
                return Unauthorized(new AuthResponseDto { Message = "Access denied. You do not have admin privileges." });
            }

            var token = _jwtHelper.GenerateToken(user, request.RememberMe);

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

        // GET: /Account/GetProfile
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

        // POST: /Account/UpdateProfile
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

                if (user.Email != request.Email)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.UserId != userId);
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

        // DELETE: /Account/DeleteAccount
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

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
                if (!isPasswordValid)
                    return BadRequest(new { message = "Current password is incorrect." });

                if (request.NewPassword != request.ConfirmPassword)
                    return BadRequest(new { message = "New password and confirmation do not match." });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password changed successfully!" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Please login to change your password." });
            }
        }

        // POST: /Account/UploadAvatar
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

        // GET: /Account/GetAddresses
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

        // POST: /Account/AddAddress
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var existingAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
                if (!existingAddresses.Any())
                    request.IsDefault = true;

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

        // PUT: /Account/UpdateAddress/{id}
        [HttpPut]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
                if (address == null)
                    return NotFound(new { message = "Address not found." });

                if (request.IsDefault && !address.IsDefault)
                {
                    var otherAddresses = await _context.Addresses.Where(a => a.UserId == userId && a.AddressId != id).ToListAsync();
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

        // DELETE: /Account/DeleteAddress/{id}
        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
                if (address == null)
                    return NotFound(new { message = "Address not found." });

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                if (address.IsDefault)
                {
                    var newDefault = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
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

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out" });
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string token, string otp)
        {
            ViewData["Token"] = token;
            ViewData["Otp"] = otp;
            return View();
        }
    }
}