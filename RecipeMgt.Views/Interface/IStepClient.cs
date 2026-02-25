using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IStepClient
    {
        Task<List<StepResponse>> GetByRecipeIdAsync(int recipeId);
        Task<StepResponse?> GetByIdAsync(int id);
        Task<CreateStepResponse> CreateAsync(int recipeId, int stepNumber, string instruction);
        Task<DeleteStepResponse> DeleteAsync(int id);
        Task<UpdateStepResponse> UpdateAsync(int stepId, int stepNumber, string instruction);
    }
}
