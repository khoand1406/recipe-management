using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IDashboardClient
    {
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();
        Task<ApiResponse<List<DishResponse>>> GetDishResponsesAsync();
        Task<ApiResponse<List<DishResponse>>> GetTopDishViewAsync();
        Task<ApiResponse<List<DishResponse>>> GetLatestDishesAsync();
        Task<ApiResponse<List<UserResponse>>> GetTopContributorAsync();
    }
}
