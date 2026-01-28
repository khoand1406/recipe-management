using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class RecipeStatistic
    {
        public int RecipeId { get; set; }

        public int ViewCount { get; set; }
        public int BookmarkCount { get; set; }
        public int CommentCount { get; set; }

        public double AvgRating { get; set; }
        public int RatingCount { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public Recipe Recipe { get; set; }
    }
}
