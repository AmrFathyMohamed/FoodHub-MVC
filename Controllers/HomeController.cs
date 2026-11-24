using FoodHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace FoodHub.Controllers
{
    public class HomeController : Controller
    {
        // GET /  أو  /Home/Index
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            ViewData["Tagline"] = "Order from the best restaurants in Assiut";
            return View();   // → Views/Home/Index.cshtml
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
