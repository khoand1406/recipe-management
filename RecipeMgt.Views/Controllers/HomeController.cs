using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.ViewModels;
using RecipeMgt.Views.Services;
using System.Diagnostics;

namespace RecipeMgt.Views.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardClient _client;

        public HomeController(ILogger<HomeController> logger, IDashboardClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IActionResult> Index()
        {

            var categories= _client.GetCategoriesAsync();
            var topDishes= _client.GetTopDishViewAsync();
            var topUsers= _client.GetTopContributorAsync();

            await Task.WhenAll(categories, topDishes, topUsers);

            var models = new HomeViewModel
            {
                Categories = categories.Result,
                TopDishes = topDishes.Result,
                TopUsers = topUsers.Result

            };
            return View(models);
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
