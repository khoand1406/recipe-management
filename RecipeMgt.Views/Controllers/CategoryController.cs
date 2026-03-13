using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using System.Threading.Tasks;

namespace RecipeMgt.Views.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IDashboardClient _client;

        public CategoryController(IDashboardClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result= await _client.GetCategoriesAsync();
            return View(result);
        }
    }
}
