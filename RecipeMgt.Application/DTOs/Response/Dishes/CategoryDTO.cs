using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Dishes
{
    public class CategoryDTO
    {
        public int CategoryId;

        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public List<DishBasicResponse> Dishes { get; set; } = [];
    }

    public class DishBasicResponse
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public string[]? Images { get; set; }
        public int CategoryId { get; set; }
    }
}
