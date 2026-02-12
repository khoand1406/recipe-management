using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IDashboardClient
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<DishResponse>> GetDishResponsesAsync();
        Task<List<DishResponse>> GetTopDishViewAsync();
        Task<List<DishResponse>> GetLatestDishesAsync();
        Task<List<UserResponse>> GetTopContributorAsync();
    }
}
