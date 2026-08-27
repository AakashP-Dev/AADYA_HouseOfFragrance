using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AADYA_HouseOfFragrance.Controllers
{
    /// <summary>
    /// Handles administrative views including the admin login and dashboard.
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        /// GET: /Admin/Login
        /// Displays the admin login page. If the user is already authenticated
        /// as an admin, they are redirected directly to the dashboard.
        /// </summary>
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If the current user is already authenticated as an admin,
            // skip the login page and go straight to the dashboard.
            if (User.Identity is { IsAuthenticated: true } && User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        /// <summary>
        /// GET: /Admin/Dashboard
        /// Displays the admin dashboard. Access is restricted to users who
        /// have logged in via the admin login flow (session flag is set).
        /// </summary>
        public IActionResult Dashboard()
        {
            // Check if the admin session flag has been set.
            var isAdmin = HttpContext.Session.GetString("AdminLoggedIn") == "true";

            if (!isAdmin)
            {
                // Redirect to the standard login page if not authenticated as admin.
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}