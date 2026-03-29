using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;

namespace RecipeMgt.Views.Models.ViewModels
{
    public class DishesViewModel
    {
        public IEnumerable<CategoryDishResponse> Dishes { get; set; }= Array.Empty<CategoryDishResponse>();
        public IEnumerable<CategoryDTO> Categories { get; set; } = Array.Empty<CategoryDTO>();    
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public string? SearchQuery { get; set; }
        public int? CategoryFilter { get; set; }
        public int? CategoryIdFilter { get; internal set; }
    }
}
