using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> getAll()
        {
            var result = await _dishService.getDishes();
            return Ok(result);
        }

        [HttpGet("cate/{id}")]
        public async Task<IActionResult> getListByCategory(int id)
        {
            var result= await _dishService.getDishesByCategory(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getDetail(int id)
        {
            var result= await _dishService.GetDishDetail(id);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> createDishes([FromBody] CreateDishRequest request)
        {
            try
            {
                var result= await _dishService.CreateDish(request);
                if (result.Success)
                {
                    return Ok(result.Data);
                }
                return BadRequest(result.Message);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update")]

        public async Task<IActionResult> updateDishes([FromBody] UpdateDishRequest request)
        {
            try
            {
                var result= await _dishService.UpdateDish(request);
                if (result.Success)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> deleteDishes(int id)
        {
            var result = await _dishService.deleteDish(id);
            if(result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

    }
}
