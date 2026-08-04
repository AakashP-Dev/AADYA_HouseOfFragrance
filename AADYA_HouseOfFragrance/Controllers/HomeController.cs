using AADYA_HouseOfFragrance.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AADYA_HouseOfFragrance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /
        // GET: /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Catalog
        public IActionResult Catalog()
        {
            return View();
        }

        // GET: /Home/Details?name=Black%20Oud
        public IActionResult Details(string name)
        {
            ViewData["ProductName"] = string.IsNullOrEmpty(name) ? "Black OUD" : name;
            return View();
        }

        // GET: /Home/Cart
        public IActionResult Cart()
        {
            return View();
        }

        // GET: /Home/Wizard
        public IActionResult Wizard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}