#nullable disable

using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Handles the main views and navigation pages of the application,
    /// including the homepage, catalog, cart, wizard, payment, and order confirmation.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ===================== MAIN VIEWS =====================

        /// <summary>GET: /</summary>
        public IActionResult Index() => View();

        /// <summary>GET: /Home/Catalog</summary>
        public IActionResult Catalog() => View();

        /// <summary>GET: /Home/Details?name=Black%20Oud</summary>
        public IActionResult Details(string name)
        {
            ViewData["ProductName"] = string.IsNullOrEmpty(name) ? "Black OUD" : name;
            return View();
        }

        /// <summary>GET: /Home/Cart</summary>
        public IActionResult Cart() => View();

        /// <summary>GET: /Home/Wizard</summary>
        public IActionResult Wizard() => View();

        /// <summary>GET: /Home/Payment</summary>
        public IActionResult Payment() => View();

        /// <summary>GET: /Home/Privacy</summary>
        public IActionResult Privacy() => View();

        /// <summary>
        /// GET: /Home/Error
        /// Displays the error page with the request ID for debugging.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // ===================== ORDER CONFIRMATION =====================

        /// <summary>
        /// GET: /Home/OrderConfirmation
        /// Displays the order confirmation page for a specific order.
        /// Uses session or JWT claims to authenticate the user.
        /// Only allows the order owner to view their own order.
        /// </summary>
        /// <param name="orderId">The ID of the order to display.</param>
        public IActionResult OrderConfirmation(int orderId)
        {
            // Try to get user ID from session (server-side authentication)
            int? userId = HttpContext.Session.GetInt32("UserId");

            // If session is null, fall back to JWT claims (API authentication)
            if (userId == null)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = int.Parse(userIdClaim.Value);
                    // Store in session for future requests
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                }
            }

            // If still null, redirect to login page
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the order with all related data
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