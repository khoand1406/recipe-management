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
        public string DishName { get; set; }= string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; } = 0;


    }

    public class UpdateDishRequest
    {
        public int DishId { get; set; }
        public string Name { get; set; } = string.Empty;
       
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
