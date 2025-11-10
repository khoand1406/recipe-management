using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Request.Ingredients;
using RecipeMgt.Application.Services.Ingredients;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly ILogger<IngredientController> _logger;

        public IngredientController(IIngredientService ingredientService, ILogger<IngredientController> logger)
        {
            _ingredientService = ingredientService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _ingredientService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Ingredient with id {id} not found" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ingredient by id");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("recipe/{recipeId:int}")]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            try
            {
                var result = await _ingredientService.GetByRecipeIdAsync(recipeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ingredients by recipe id");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateIngredientRequest request)
        {
            try
            {
                var result = await _ingredientService.CreateAsync(request);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ingredient");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateIngredientRequest request)
        {
            try
            {
                var result = await _ingredientService.UpdateAsync(request);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ingredient");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _ingredientService.DeleteAsync(id);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ingredient");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
