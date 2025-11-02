using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Recipes
{
    public class UpdateRecipeRequest
    {
        public int RecipeId { get; set; }

        public int DishId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public int? CookingTime { get; set; }

        public int? Servings { get; set; }

        public string DifficultyLevel { get; set; } = "Medium";

        public List<IFormFile>? Images { get; set; }

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
}
