using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Rating
{
    public class AddRatingRequest
    {
        [Required]
        public int RecipeId { get; set; }
        [Required]
        public int Score { get; set; }  // 1-5
        [Required]
        public string? Comment { get; set; }

    }
}
