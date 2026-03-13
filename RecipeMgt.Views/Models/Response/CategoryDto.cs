using RecipeMgt.Application.DTOs.Response.Dishes;

namespace RecipeMgt.Views.Models.Response
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int DishesCount { get; set; }

        public int AuthorCount { get; set; }
    }

    }

