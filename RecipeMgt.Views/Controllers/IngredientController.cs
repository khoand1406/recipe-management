using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class IngredientController : Controller
    {
        private readonly ILogger<IngredientController> _logger;
        private readonly IIngredientClient _ingredientClient;
        

        public IngredientController(ILogger<IngredientController> logger, IIngredientClient client)
        {
            _logger = logger;
            _ingredientClient = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientClient.GetByIdAsync(id);
            if (ingredient == null) return NotFound();
            return Json(ingredient);
        }

        [HttpGet]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            var ingredients = await _ingredientClient.GetByRecipeIdAsync(recipeId);
            return Json(ingredients);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int recipeId, string name, string quantity)
        {
            var result = await _ingredientClient.CreateAsync(recipeId, name, quantity);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int ingredientId, string name, string quantity)
        {
            var result = await _ingredientClient.UpdateAsync(ingredientId, name, quantity);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ingredientClient.DeleteAsync(id);
            return Json(result);
        }
    }
}
