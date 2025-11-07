using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.Services.Dishes;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly ILogger<DishController> _logger;

        public DishController(IDishService dishService, ILogger<DishController> logger)
        {
            _dishService = dishService;
            _logger = logger;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dishService.getDishes();
            return Ok(result);
        }

        
        [HttpGet("cate/{id:int}")]
        public async Task<IActionResult> GetListByCategory(int id)
        {
            var result = await _dishService.getDishesByCategory(id);
            return Ok(result);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _dishService.GetDishDetail(id);
            if (result == null)
                return NotFound($"Không tìm thấy món ăn có ID = {id}");

            return Ok(result);
        }

        
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDish([FromForm] CreateDishRequest request)
        {
            try
            {
                var result = await _dishService.CreateDish(request);
                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dish");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        
        [HttpPatch("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDish([FromForm] UpdateDishRequest request)
        {
            try
            {
                var result = await _dishService.UpdateDish(request);
                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dish");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try
            {
                var result = await _dishService.deleteDish(id);
                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dish");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
