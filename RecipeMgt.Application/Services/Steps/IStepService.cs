using RecipeMgt.Application.DTOs.Request.Steps;
using RecipeMgt.Application.DTOs.Response.Steps;

namespace RecipeMgt.Application.Services.Steps
{
    public interface IStepService
    {
        Task<StepResponse?> GetByIdAsync(int stepId);
        Task<IEnumerable<StepResponse>> GetByRecipeIdAsync(int recipeId);
        Task<CreateStepResponse> CreateAsync(CreateStepRequest request);
        Task<UpdateStepResponse> UpdateAsync(UpdateStepRequest request);
        Task<DeleteStepResponse> DeleteAsync(int stepId);
    }
}
