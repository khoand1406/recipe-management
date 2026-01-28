using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class UserStatistic
    {
        public int UserId { get; set; }

        public int RecipeCount { get; set; }
        public int CommentCount { get; set; }
        public int RatingCount { get; set; }
        public int FollowerCount { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public User User { get; set; } = new User();
    }
}
