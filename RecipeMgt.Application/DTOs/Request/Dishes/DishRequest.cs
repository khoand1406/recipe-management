using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Dishes
{
    public class DishRequest
    {

    }

    public class CreateDishRequest
    {
        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Dish name must be between 2 and 100 characters.")]
        public string DishName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AuthorId must be a positive integer.")]
        public int? AuthorId { get; set; }

        public List<IFormFile>? Images { get; set; }

    }

    public class UpdateDishRequest
    {
        [Required(ErrorMessage = "Dish ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "DishId must be a positive integer.")]
        public int DishId { get; set; }

        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Dish name must be between 2 and 100 characters.")]
        public string DishName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive integer.")]
        public int CategoryId { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
