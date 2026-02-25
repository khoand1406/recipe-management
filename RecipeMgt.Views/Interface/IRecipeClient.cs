using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IRecipeClient
    {
        void SetBearerToken(string token);
        Task<ApiResponse<List<RecipeResponse>>> GetByDishAsync(int dishId);
        Task<ApiResponse<RecipeResponse>> GetByIdAsync(int id);

        Task<ApiResponse<List<RecipeWithUserInfo>>> GetMineAsync();

        Task<ApiResponse<List<RecipeResponse>>> GetByFilterAsync();
        
        Task<ApiResponse<PagedResponse<RecipeResponse>>> SearchAsync(object query);

        Task<ApiResponse<UploadImageResult>> UploadImageAsync(Stream fileStreamm, string fileName, string contentType);
        Task<ApiResponse<CreateRecipeResponse>> CreateAsync(MultipartFormDataContent form);
        Task <ApiResponse<DeleteRecipeResponse>> DeleteAsync(int id);

        Task<bool> AddCommentAsync(int recipeId, string content);

        Task<bool> AddBookmarkAsync(int recipeId);

        Task<ApiResponse<List<CommentResponseDTO>>> GetCommentsAsync(int recipeId); 
        Task<ApiResponse<UpdateRecipeResponse>> UpdateAsync(MultipartFormDataContent form);
    }
}
