using System.Text.Json.Serialization;

namespace RecipeMgt.Views.Models.Response
{
    public class RecipeResponse
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int? CookingTime { get; set; }
        public int? Servings { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Images { get; set; } = new();
        public UserBasicResponse Author { get; set; } = new();
    }

    public class CreateRecipeResponse
    {
        public bool Success { get; set; }
        public RecipeResponse Data { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateRecipeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteRecipeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RecipeWithUserInfo
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? CookingTime { get; set; }
        public int? Servings { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public UserBasicResponse Author { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserBasicResponse
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

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
}
