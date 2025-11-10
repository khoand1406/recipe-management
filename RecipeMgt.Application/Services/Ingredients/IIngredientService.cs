using RecipeMgt.Application.DTOs.Request.Ingredients;
using RecipeMgt.Application.DTOs.Response.Ingredients;

namespace RecipeMgt.Application.Services.Ingredients
{
    public interface IIngredientService
    {
        Task<IngredientResponse?> GetByIdAsync(int ingredientId);
        Task<IEnumerable<IngredientResponse>> GetByRecipeIdAsync(int recipeId);
        Task<CreateIngredientResponse> CreateAsync(CreateIngredientRequest request);
        Task<UpdateIngredientResponse> UpdateAsync(UpdateIngredientRequest request);
        Task<DeleteIngredientResponse> DeleteAsync(int ingredientId);
    }
}
