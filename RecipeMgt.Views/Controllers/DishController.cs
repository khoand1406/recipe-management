using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models.Response;
using RecipeMgt.Views.Services;
using System.Net.Http.Headers;
using System.Text;

namespace RecipeMgt.Views.Controllers
{
    public class DishController : Controller
    {
        private readonly ILogger<DishController> _logger;
        private readonly IDishClient _dishClient;

        public DishController(ILogger<DishController> logger, IDishClient dishClient)
        {
            _logger = logger;
            _dishClient = dishClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dishes = await _dishClient.GetAllAsync();
            return View(dishes);
        }

        [HttpGet]
        public async Task<IActionResult> ByCategory(int id)
        {
            var dishes = await _dishClient.GetByCategoryAsync(id);
            ViewBag.CategoryId = id;
            return View(dishes);
        }

        [HttpGet]
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
            var resp = await _dishClient.DeleteAsync(id);
            return Json(resp);
        }
    }
}
