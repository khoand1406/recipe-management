using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public string DishName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public List<IFormFile>? Images { get; set; }
        // Danh sách ảnh (nếu có)
        
    }

    public class UpdateDishRequest
    {
        public int DishId { get; set; }

        public string DishName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public List<IFormFile>? Images { get; set; }

        // Cho phép thay đổi hoặc thêm ảnh
        
    }
}
