using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Request.Steps;
using RecipeMgt.Application.Services.Steps;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepController : ControllerBase
    {
        private readonly IStepService _stepService;
        private readonly ILogger<StepController> _logger;

        public StepController(IStepService stepService, ILogger<StepController> logger)
        {
            _stepService = stepService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _stepService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Step with id {id} not found" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting step by id");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("recipe/{recipeId:int}")]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            try
            {
                var result = await _stepService.GetByRecipeIdAsync(recipeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting steps by recipe id");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateStepRequest request)
        {
            try
            {
                var result = await _stepService.CreateAsync(request);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating step");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateStepRequest request)
        {
            try
            {
                var result = await _stepService.UpdateAsync(request);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating step");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _stepService.DeleteAsync(id);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting step");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
