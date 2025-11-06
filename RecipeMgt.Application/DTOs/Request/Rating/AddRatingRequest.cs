using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Rating
{
    public class AddRatingRequest
    {
        public int RecipeId { get; set; }
        public int Score { get; set; }  // 1-5
        public string? Comment { get; set; }

    }
}
