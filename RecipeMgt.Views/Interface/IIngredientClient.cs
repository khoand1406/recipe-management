
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IIngredientClient
    {
        public Task<ApiResponse<IngredientResponse>> GetByIdAsync(int ingredientId);

        public Task<List<IngredientResponse>> GetByRecipeIdAsync(int recipeId);

        public Task<ApiResponse<CreateIngredientResponse>> CreateAsync(int recipeId, string name, string quantity);

        public Task<ApiResponse<UpdateIngredientResponse>> UpdateAsync(int ingredientId, string name, string quantity);

        public Task<ApiResponse<DeleteIngredientResponse>> DeleteAsync(int ingredientId);

    }
}
