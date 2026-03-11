using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;
using RecipeMgt.Views.Models.ViewModels;
using RecipeMgt.Views.Services;
using System.Net.Http.Headers;
using System.Text;

namespace RecipeMgt.Views.Controllers
{
    public class DishController : Controller
    {
        private readonly ILogger<DishController> _logger;
        private readonly IDishClient _dishClient;
        private readonly IDashboardClient _dashboardClient;

        public DishController(ILogger<DishController> logger, IDishClient dishClient, IDashboardClient dashboardClient)
        {
            _logger = logger;
            _dishClient = dishClient;
            _dashboardClient = dashboardClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dishes = await _dishClient.GetAllAsync();
            return View(dishes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ByCategory(
     int? id,
     int page = 1,
     string? searchQuery = null)
        {
            var dishesTask = _dishClient.GetDishesAsync(page, 10, searchQuery, id);
            var categoryTask = _dashboardClient.GetCategoriesAsync();

            await Task.WhenAll(dishesTask, categoryTask);

            var dishesResult = await dishesTask;
            var categories = await categoryTask;

            var category = categories?.Data?
                .FirstOrDefault(x => x.CategoryId == id);

            if (dishesResult == null)
            {
                return View(new CategoryDishViewModel());
            }

            var response = new CategoryDishViewModel
            {
                CategoryId = id,
                SearchQuery = searchQuery,

                CategoryName = category?.CategoryName,
                CategoryImage = category?.ImageUrl ?? "",
                Description = category?.Description ?? "",

                Dishes = dishesResult.Items.ToList(),

                CurrentPage = dishesResult.Page,
                TotalPages = dishesResult.TotalPages,
                TotalCount = dishesResult.TotalItems
            };

            return View(response);
        }

        [HttpGet()]
        public async Task<IActionResult> Detail(int id)
        {
            var dish = await _dishClient.GetDetailAsync(id);
            if (dish == null) return NotFound();
            return View(dish);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string dishName, string description, int categoryId, List<IFormFile>? images)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(dishName ?? string.Empty, Encoding.UTF8), "DishName");
            form.Add(new StringContent(description ?? string.Empty, Encoding.UTF8), "Description");
            form.Add(new StringContent(categoryId.ToString()), "CategoryId");
            if (images != null)
            {
                foreach (var file in images)
                {
                    using var s = file.OpenReadStream();
                    var fileContent = new StreamContent(s);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(fileContent, "Images", file.FileName);
                }
            }
            var resp = await _dishClient.CreateAsync(form);
            return Json(resp);
        }

        [HttpPatch]
        public async Task<IActionResult> Update(int dishId, string dishName, string description, int categoryId, List<IFormFile>? images)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(dishId.ToString()), "DishId");
            form.Add(new StringContent(dishName ?? string.Empty, Encoding.UTF8), "DishName");
            form.Add(new StringContent(description ?? string.Empty, Encoding.UTF8), "Description");
            form.Add(new StringContent(categoryId.ToString()), "CategoryId");
            if (images != null)
            {
                foreach (var file in images)
                {
                    using var s = file.OpenReadStream();
                    var fileContent = new StreamContent(s);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(fileContent, "Images", file.FileName);
                }
            }
            var resp = await _dishClient.UpdateAsync(form);
            return Json(resp);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
             await _dishClient.DeleteAsync(id);
            return View();
        }
    }
}
