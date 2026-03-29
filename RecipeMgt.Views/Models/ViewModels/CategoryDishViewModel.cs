using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Models.ViewModels
{
    public class CategoryDishViewModel
    {
        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? SearchQuery { get; set; }

        public List<CategoryDishResponse> Dishes { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        public string CategoryImage { get; internal set; }
        public string Description { get; internal set; }
    }
}
