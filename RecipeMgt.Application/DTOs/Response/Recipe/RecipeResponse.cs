using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Recipe
{
    public class RecipeResponse
    {
        public int RecipeId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        public int? CookingTime { get; set; }

        public int? Servings { get; set; }

        public string DifficultyLevel { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual RecipeMgt.Domain.Entities.User Author { get; set; } = new RecipeMgt.Domain.Entities.User();

        public ICollection<Image> Images { get; set; }= new List<Image>();
    }

    public class CreateRecipeResponse
    {
        public bool Success { get; set; } = false;

        public RecipeResponse Data { get; set; } = new RecipeResponse();

        public string Message { get; set; } = string.Empty;
    }

    public class UpdateRecipeResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public class DeleteRecipeResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public class RecipeWithUserInfo
    {
        public int RecipeId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int? CookingTime { get; set; }

        public int? Servings { get; set; }

        public string DifficultyLevel { get; set; } = string.Empty;

        public UserBasicResponse Author { get; set; }= new UserBasicResponse();

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


    }

    public class UserBasicResponse
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;


    }
}
