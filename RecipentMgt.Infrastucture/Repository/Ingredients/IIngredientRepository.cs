using RecipeMgt.Domain.Entities;

namespace RecipentMgt.Infrastucture.Repository.Ingredients
{
    public interface IIngredientRepository
    {
        Task<Ingredient?> GetByIdAsync(int ingredientId);
        Task<IEnumerable<Ingredient>> GetByRecipeIdAsync(int recipeId);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient?> UpdateAsync(Ingredient ingredient);
        Task<bool> DeleteAsync(int ingredientId);

        void RemoveRange(IEnumerable<Ingredient> ingredients);

        Task AddRangeAsync(IEnumerable<Ingredient>ingredients);

        void UpdateRange(IEnumerable<Ingredient> ingredients);


    }
}
