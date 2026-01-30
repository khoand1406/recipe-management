using RecipeMgt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Statistics.Recipe
{
    public interface IStatisticService
    {
        public Task RecipeViewd(int recipeId);

        public Task RecipeComment(int recipeId, int userId);

        public Task RecipeBookmark(int recipeId);

        public Task RecipeRemoveBookmark(int recipeId);

        public Task RecipeRated(int recipeId, int userId, int recipeRating);
    }
}
