using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Management.Dashboard
{
    public class DashboardResponse
    {
    }

    public class DashboardMetricResponse
    {
        public int TotalDish {  get; set; }

        public int TotalUser {  get; set; }

        public int TotalRecipes { get; set; }

        public int TotalReviews { get; set; }
    }

    public class ChartCategoryDishResponse
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int DishCount { get; set; }
    }

    public class ChartDishCreateResponse
    {
        public int Month { get; set; }

        public int RecipeCount { get; set; }
    }

    public class ChartTopRecipeRatingResponse
    {
        public int RecipeId { get; set; }

        public string RecipeName { get; set; }

        public double RecipeRating { get; set; }


    }
}
