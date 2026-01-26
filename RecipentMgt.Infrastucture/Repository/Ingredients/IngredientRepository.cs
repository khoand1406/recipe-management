using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;

namespace RecipentMgt.Infrastucture.Repository.Ingredients
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly RecipeManagementContext _context;

        public IngredientRepository(RecipeManagementContext context)
        {
            _context = context;
        }

        public async Task<Ingredient?> GetByIdAsync(int ingredientId)
        {
            return await _context.Ingredients.FindAsync(ingredientId);
        }

        public async Task<IEnumerable<Ingredient>> GetByRecipeIdAsync(int recipeId)
        {
            return await _context.Ingredients
                .Where(i => i.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient?> UpdateAsync(Ingredient ingredient)
        {
            var existing = await _context.Ingredients.FindAsync(ingredient.IngredientId);
            if (existing == null) return null;

            existing.Name = ingredient.Name;
            existing.Quantity = ingredient.Quantity;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null) return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public void RemoveRange(IEnumerable<Ingredient> ingredients)=> _context.RemoveRange(ingredients);

        public async Task AddRangeAsync(IEnumerable<Ingredient> ingredients)
        {
            await _context.AddRangeAsync(ingredients);
        }

        public void UpdateRange(IEnumerable<Ingredient> ingredients)=> _context.UpdateRange(ingredients);
        
    }
}
