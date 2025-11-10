namespace RecipeMgt.Views.Models.Response
{
    public class CommentResponseDTO
    {
        public int CommentId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AddCommentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
