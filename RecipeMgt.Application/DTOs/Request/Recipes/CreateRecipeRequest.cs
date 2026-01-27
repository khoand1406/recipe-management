using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RecipeMgt.Application.DTOs.Request.Recipes
{
    public class CreateRecipeRequest
    {
        public int DishId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public int? CookingTime { get; set; }

        public int? Servings { get; set; }

        public string DifficultyLevel { get; set; } = "Medium";

        public List<string> ImageUrls { get; set; } = [];

        public List<IngredientDto> Ingredients { get; set; } = [];

        public List<StepDto> Steps { get; set; } = [];
    }


    public class IngredientDto
    {
        public int? IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }

    public class StepDto
    {
        public int? StepId { get; set; }
        public int StepNumber { get; set; }

        public string Instruction { get; set; } = string.Empty;
    }
}
