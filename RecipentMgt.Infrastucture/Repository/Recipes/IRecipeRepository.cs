using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Recipes
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> getAll();

        Task<IEnumerable<Recipe>> GetRecipes(int dishId);

        Task<IEnumerable<Recipe>> getRecipesByFilter();

        public Task<IEnumerable<Recipe>> GetRecipesByUser(int userId);

        public Task<IEnumerable<Recipe>> GetRelatedRecipes(int id);

        Task<Recipe?> getRecipeById(int id);

        public Task<PagedResponse<Recipe>> GetSearchedResult(SearchRecipeRequest search);

        Task AddAsync(Recipe newRecipe);

        Task AddRangeAsync(List<Image> images);

        Task AddRangeAsync(List<Ingredient> ingredients);

        Task AddRangeAsync(List<Step> steps);

       

        void RemoveRange(List<Image> images);

        void Update(Recipe newRecipe);
        Task<bool> deleteRecipes(int id);
        Task<List<Image>> getRecipeImages(int recipeId);
    }
}
