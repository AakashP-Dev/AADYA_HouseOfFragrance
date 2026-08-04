using Microsoft.AspNetCore.Mvc;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class AdminController : Controller
    {
        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}