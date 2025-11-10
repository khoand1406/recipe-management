namespace RecipeMgt.Views.Models.Response
{
    public class BookmarkResponseDto
    {
        public int BookmarkId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string? RecipeThumbnail { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddBookmarkResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
