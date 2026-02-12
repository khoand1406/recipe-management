using RecipeMgt.Domain.Entities;

namespace RecipeMgt.Views.Models.Response
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

        public int ViewCount { get; set; }

        public int BookmarkCount { get; set; }
    }

    public class DishDetailResponse
    {
        public int DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public List<RecipeResponse> Recipes { get; set; } = new();
    }

    public class CreateDishResponse
    {
        public bool Success { get; set; }
        public DishResponse Data { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateDishResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteDishResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
