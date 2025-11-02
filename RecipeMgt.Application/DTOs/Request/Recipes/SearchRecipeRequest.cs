using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Recipes
{
    public class SearchRecipeRequest
    {
        public string? Title { get; set; }
        public string? Ingredient { get; set; }
        public string? Difficulty { get; set; }
        public int? MaxCookingTime { get; set; }
        public string? CreatorName { get; set; }

        // ✅ Mới thêm:
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt"; // Field sắp xếp
        public string? SortOrder { get; set; } = "desc";   // asc hoặc desc
    }
}
