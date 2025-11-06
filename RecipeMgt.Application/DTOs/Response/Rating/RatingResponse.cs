using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Rating
{
    public class RatingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }

    public class UserRatingResponse
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
