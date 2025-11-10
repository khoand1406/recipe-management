using RecipeMgt.Domain.Entities;

namespace RecipentMgt.Infrastucture.Repository.Steps
{
    public interface IStepRepository
    {
        Task<Step?> GetByIdAsync(int stepId);
        Task<IEnumerable<Step>> GetByRecipeIdAsync(int recipeId);
        Task<Step> CreateAsync(Step step);
        Task<Step?> UpdateAsync(Step step);
        Task<bool> DeleteAsync(int stepId);
    }
}
