using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Constant
{
    public static class RecipeErrorMessage
    {
        public const string NotFound = "RECIPE_NOT_FOUND";
        public const string DishNotFound = "DISH_NOT_FOUND";
        public const string CreateFailed = "CREATE_RECIPE_FAILED";
        public const string UpdateFailed = "UPDATE_RECIPE_FAILED";
        public const string DeleteFailed = "DELETE_RECIPE_FAILED";

        public const string Forbidden = "ACTION_FORBIDDEN";
    }
}
