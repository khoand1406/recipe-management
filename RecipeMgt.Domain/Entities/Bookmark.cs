using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class Bookmark
    {
        public int BookmarkId { get; set; }

        public int UserId { get; set; }
        public int RecipeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = new User();
        public Recipe Recipe { get; set; }= new Recipe();
    }
}
