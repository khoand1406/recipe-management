using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }
        public int RecipeId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; }= new User();
        public Recipe Recipe { get; set; }= new Recipe();
    }
}
