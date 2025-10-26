using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Recipes
{
    public interface IRecipeServices
    {
        public Task<IEnumerable<RecipeResponse>> GetRecipesByFilter();

        public Task<IEnumerable<RecipeResponse>> GetRecipesByDish(int id);

        public Task<IEnumerable<RecipeWithUserInfo>> GetRecipesByUser(int userId);

        public Task<IEnumerable<RecipeResponse>> GetRelatedRecipes(int id);

        public Task<CreateRecipeResponse> CreateRecipe(CreateRecipeRequest request);

        public Task<UpdateRecipeResponse> UpdateRecipe(UpdateRecipeRequest request);

        public Task<DeleteRecipeResponse> DeleteRecipe(int id);


    }
}
