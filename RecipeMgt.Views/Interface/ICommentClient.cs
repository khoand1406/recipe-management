using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Comments;
using RecipeMgt.Views.Models.Response;


namespace RecipeMgt.Views.Interface
{
    public interface ICommentClient
    {
        public void SetBearerToken(string token);   

        public Task<ApiResponse<List<CommentResposneDTO>>> GetCommentsByRecipeIdAsync(int recipeId);
        
        public Task<AddCommentResponse> AddCommentAsync(int recipeId, string content);

        
    }
}
