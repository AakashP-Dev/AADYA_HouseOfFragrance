#nullable disable

using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();
        public IActionResult Catalog() => View();
        public IActionResult Details(string name)
        {
            ViewData["ProductName"] = string.IsNullOrEmpty(name) ? "Black OUD" : name;
            return View();
        }
        public IActionResult Cart() => View();
        public IActionResult Wizard() => View();
        public IActionResult Payment() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ✅ FIXED: Order Confirmation – uses session AND claims
        public IActionResult OrderConfirmation(int orderId)
        {
            // Try to get user ID from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            // If session is null, try from the authenticated user (if using cookie or JWT)
            if (userId == null)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = int.Parse(userIdClaim.Value);
                    // Store in session for next time
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                }
            }

            // If still null, redirect to login
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}