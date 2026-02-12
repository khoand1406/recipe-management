using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class StepController : Controller
    {
        private readonly ILogger<StepController> _logger;
        private readonly StepClient _stepClient;
        private readonly IConfiguration _configuration;

        public StepController(ILogger<StepController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            _stepClient = new StepClient(apiBaseUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var step = await _stepClient.GetByIdAsync(id);
            if (step == null) return NotFound();
            return Json(step);
        }

        [HttpGet]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            var steps = await _stepClient.GetByRecipeIdAsync(recipeId);
            return Json(steps);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int recipeId, int stepNumber, string instruction)
        {
            var result = await _stepClient.CreateAsync(recipeId, stepNumber, instruction);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int stepId, int stepNumber, string instruction)
        {
            var result = await _stepClient.UpdateAsync(stepId, stepNumber, instruction);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stepClient.DeleteAsync(id);
            return Json(result);
        }
    }
}
