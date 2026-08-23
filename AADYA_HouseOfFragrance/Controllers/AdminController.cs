using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class AdminController : Controller
    {
        // GET: /Admin/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If already logged in as admin, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            var isAdmin = HttpContext.Session.GetString("AdminLoggedIn") == "true";
            if (!isAdmin)
            {
                // Optionally redirect to normal login page
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}