using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Comments
{
    public class CommentResposneDTO
    {
        public int CommentId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string? UserAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class BookmarkResponseDto
    {
        public int BookmarkId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string? RecipeThumbnail { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
