using Microsoft.AspNetCore.Mvc;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class AccountController : Controller
    {
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
    }
}