using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RecipeMgt.Application.DTOs.Request.Recipes
{
    public class CreateRecipeRequest
    {
        public int RecipeId { get; set; }

        public int DishId { get; set; }

        public string Title { get; set; }=string.Empty;

        public string Description { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public int? CookingTime { get; set; }

        public int? Servings { get; set; }

        public string DifficultyLevel { get; set; } = "Medium";


        public List<string>? ImageUrls
        {
            get; set;
        }

        [FromForm]
        public string IngredientsJson { get; set; } = string.Empty;

        [FromForm]
        public string StepsJson { get; set; } = string.Empty;

        [NotMapped]
        public List<IngredientDto> Ingredients { get; set; } = new();

        [NotMapped]
        public List<StepDto> Steps { get; set; } = new();
    }

    public class IngredientDto
    {
        public int? IngredientId { get; set; } // null = mới
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }

    public class StepDto
    {
        public int? StepId { get; set; } // null = mới
        public int StepNumber { get; set; }

        public string Instruction { get; set; } = string.Empty;
    }
}
