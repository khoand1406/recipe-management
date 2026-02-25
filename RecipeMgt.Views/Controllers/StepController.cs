using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class StepController : Controller
    {
        private readonly ILogger<StepController> _logger;
        private readonly IStepClient _stepClient;
        

        public StepController(ILogger<StepController> logger, IStepClient stepClient)
        {
            _logger = logger;
            _stepClient = stepClient;
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
