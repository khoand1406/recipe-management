using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.RequestModel;
using System.Diagnostics;

namespace RecipeMgt.Views.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DashboardClient _client;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _client= new DashboardClient("https://localhost:7059");
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _client.GetCategoriesAsync();
            return View(categories);
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
