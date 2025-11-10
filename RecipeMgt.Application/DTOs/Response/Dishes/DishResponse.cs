using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Dishes
{
    public class DishResponse
    {
        public int DishId { get; set; }

        public string DishName { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName => Category?.CategoryName ?? string.Empty;

        public virtual Category? Category { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }

    public class DishDetailResponse
    {
        public int DishId { get; set; }

        public string DishName { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName => Category?.CategoryName ?? string.Empty;

        public virtual Category? Category { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        public virtual ICollection<RecipeResponse> Recipes { get; set; } = new List<RecipeResponse>();
    }

    public class CreateDishResponse
    {
        public bool Success { get; set; } = false;

        public DishResponse Data { get; set; } = new DishResponse();

        public string Message { get; set; }= string.Empty;
    }

    public class UpdateDishResponse
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }

    public class DeleteDishResponse
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }

    
}
