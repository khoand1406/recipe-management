using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Management.Dashboard;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Domain.RequestEntity;
using RecipeMgt.Views.Common;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IAdminClient
    {
        // ========================= USER =========================

        Task<Models.ApiResponse<Models.Response.PagedResponse<UserResponseMgtDTO>>> GetUsers(
            string? searchQuery,
            UserStatus? status,
            int page,
            int pageSize);

        Task<Models.ApiResponse<UserDetailResponse>> GetUserDetail(int userId);

        Task<Models.ApiResponse<UserResponseDTO>> CreateUser(CreateUserRequest request);

        Task<Models.ApiResponse<BatchImportResult>> CreateUsersFromCsv(IFormFile file);

        Task<Models.ApiResponse<UserResponseDTO>> UpdateUser(int userId, UpdateUserRequest request);

        Task<Models.ApiResponse<UserResponseDTO>> DeleteUser(int userId);

        Task<Models.ApiResponse<bool>> DeleteBatchUsers(BatchUserIdsRequest request);

        Task<Models.ApiResponse<UserBasicResponse>> BanUser(int userId);

        Task<Models.ApiResponse<bool>> BanBatchUsers(BatchUserIdsRequest request);

        Task<Models.ApiResponse<bool>> DeactiveUser(int userId);

        Task<Models.ApiResponse<bool>> DeactiveBatchUsers(BatchUserIdsRequest request);

        Task<Models.ApiResponse<bool>> RecoverUser(int userId);


        // ========================= DISH =========================

        Task<Models.ApiResponse<Models.Response.PagedResponse<DishResponse>>> GetDishes(
            string? searchQuery,
            int? categoryId,
            int page,
            int pageSize);

        Task<Models.ApiResponse<DishDetailResponse>> GetDishDetail(int dishId);

        Task<Models.ApiResponse<DishResponse>> CreateDish(CreateDishRequest request);

        Task<Models.ApiResponse<DishResponse>> UpdateDish(int dishId, UpdateDishRequest request);

        Task<Models.ApiResponse<bool>> DeleteDish(int dishId);

        Task<Models.ApiResponse<bool>> ApproveDish(int dishId);

        Task<Models.ApiResponse<bool>> RejectDish(int dishId);


        // ========================= RECIPE =========================

        Task<Models.ApiResponse<Models.Response.PagedResponse<RecipeResponse>>> GetRecipes(
            int? dishId,
            int page,
            int pageSize);

        Task<Models.ApiResponse<RecipeDetailResponse>> GetRecipeDetail(int recipeId);

        Task<Models.ApiResponse<RecipeResponse>> CreateRecipe(CreateRecipeRequest request);

        Task<Models.ApiResponse<RecipeResponse>> UpdateRecipe(int recipeId, UpdateRecipeRequest request);

        Task<Models.ApiResponse<bool>> DeleteRecipe(int recipeId);


        // ========================= DASHBOARD =========================

        Task<Models.ApiResponse<DashboardMetricResponse>> GetDashboardMetrics();

        Task<Models.ApiResponse<List<DishChartResponse>>> GetDishChartMonthly();

        Task<Models.ApiResponse<List<RecipeChartResponse>>> GetRecipeChartMonthly();

        Task<Models.ApiResponse<List<ChartCategoryDishResponse>>> GetCategoryChart();


        // ========================= MEDIA =========================

        Task<Models.ApiResponse<string>> UploadImage(IFormFile file);
        Task<Models.ApiResponse<UsersStatistic>> GetUserStatistics();
    }
}
