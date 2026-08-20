using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models.DTOs;
using AADYA_HouseOfFragrance.Models.Entities;
using AADYA_HouseOfFragrance.Helpers;

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
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto { Message = "Invalid email or password." });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new AuthResponseDto { Message = "Invalid email or password." });
            }

            // Pass the RememberMe flag to token generation
            var token = _jwtHelper.GenerateToken(user, request.RememberMe);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Message = "Login Successful!"
            });
        }
    }
}