using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly DashboardClient _client;
        private readonly RecipeClient _recipeClient;
        private readonly StepClient _stepClient;
        private readonly IngredientClient _ingredientClient;
        private readonly CommentClient _commentClient;
        private readonly IConfiguration _configuration;

        public RecipeController(ILogger<RecipeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            _client = new DashboardClient(apiBaseUrl);
            _recipeClient = new RecipeClient(apiBaseUrl);
            _stepClient = new StepClient(apiBaseUrl);
            _ingredientClient = new IngredientClient(apiBaseUrl);
            _commentClient = new CommentClient(apiBaseUrl);
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _client.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var recipe = await _recipeClient.GetByIdAsync(id);
            if (recipe == null) return NotFound();
            
            var steps = await _stepClient.GetByRecipeIdAsync(id);
            ViewBag.Steps = steps;

            var ingredients = await _ingredientClient.GetByRecipeIdAsync(id);
            ViewBag.Ingredients = ingredients;

            var comments = await _commentClient.GetCommentsByRecipeIdAsync(id);
            ViewBag.Comments = comments;
            
            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> ByDish(int id)
        {
            var data = await _recipeClient.GetByDishAsync(id);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            var data = await _recipeClient.GetMineAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ByFilter()
        {
            var data = await _recipeClient.GetByFilterAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? title, string? ingredient, string? difficulty, int? maxCookingTime, string? creatorName, int page = 1, int pageSize = 10, string? sortBy = "CreatedAt", string? sortOrder = "desc")
        {
            var query = new { Title = title, Ingredient = ingredient, Difficulty = difficulty, MaxCookingTime = maxCookingTime, CreatorName = creatorName, Page = page, PageSize = pageSize, SortBy = sortBy, SortOrder = sortOrder };
            var data = await _recipeClient.SearchAsync(query);
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            if (file == null || file.Length == 0) return BadRequest();
            using var stream = file.OpenReadStream();
            var result = await _recipeClient.UploadImageAsync(stream, file.FileName, file.ContentType);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int dishId, string title, string description, int? cookingTime, int? servings, string difficultyLevel, string ingredientsJson, string stepsJson, List<IFormFile>? images)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(dishId.ToString()), "DishId");
            form.Add(new StringContent(title ?? string.Empty, Encoding.UTF8), "Title");
            form.Add(new StringContent(description ?? string.Empty, Encoding.UTF8), "Description");
            if (cookingTime.HasValue) form.Add(new StringContent(cookingTime.Value.ToString()), "CookingTime");
            if (servings.HasValue) form.Add(new StringContent(servings.Value.ToString()), "Servings");
            form.Add(new StringContent(difficultyLevel ?? "Medium", Encoding.UTF8), "DifficultyLevel");
            form.Add(new StringContent(ingredientsJson ?? "[]", Encoding.UTF8), "IngredientsJson");
            form.Add(new StringContent(stepsJson ?? "[]", Encoding.UTF8), "StepsJson");
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
            var resp = await _recipeClient.CreateAsync(form);
            return Json(resp);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int recipeId, int dishId, string title, string description, int? cookingTime, int? servings, string difficultyLevel, string ingredientsJson, string stepsJson, List<IFormFile>? images)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(recipeId.ToString()), "RecipeId");
            form.Add(new StringContent(dishId.ToString()), "DishId");
            form.Add(new StringContent(title ?? string.Empty, Encoding.UTF8), "Title");
            form.Add(new StringContent(description ?? string.Empty, Encoding.UTF8), "Description");
            if (cookingTime.HasValue) form.Add(new StringContent(cookingTime.Value.ToString()), "CookingTime");
            if (servings.HasValue) form.Add(new StringContent(servings.Value.ToString()), "Servings");
            form.Add(new StringContent(difficultyLevel ?? "Medium", Encoding.UTF8), "DifficultyLevel");
            form.Add(new StringContent(ingredientsJson ?? "[]", Encoding.UTF8), "IngredientsJson");
            form.Add(new StringContent(stepsJson ?? "[]", Encoding.UTF8), "StepsJson");
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
            var resp = await _recipeClient.UpdateAsync(form);
            return Json(resp);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            var resp = await _recipeClient.DeleteAsync(id);
            return Json(resp);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int recipeId, string content)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            var ok = await _recipeClient.AddCommentAsync(recipeId, content);
            if (!ok) return BadRequest();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddBookmark(int recipeId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _recipeClient.SetBearerToken(token);
            var ok = await _recipeClient.AddBookmarkAsync(recipeId);
            if (!ok) return BadRequest();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Comments(int recipeId)
        {
            var data = await _recipeClient.GetCommentsAsync(recipeId);
            return Json(data);
        }
    }
}