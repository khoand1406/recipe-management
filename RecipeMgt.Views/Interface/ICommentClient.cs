using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface ICommentClient
    {
        public void SetBearerToken(string token);   

        public Task<List<CommentResponseDTO>> GetCommentsByRecipeIdAsync(int recipeId);
        
        public Task<AddCommentResponse> AddCommentAsync(int recipeId, string content);

        
    }
}
