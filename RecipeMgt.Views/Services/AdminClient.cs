using Azure.Core;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Management.Dashboard;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Domain.RequestEntity;
using RecipeMgt.Views.Common;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;
using System.Drawing.Printing;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class AdminClient : IAdminClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public AdminClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
        private async Task<T?> Deserialize<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        private void AddAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        public Task<Models.ApiResponse<bool>> ApproveDish(int dishId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<bool>> BanBatchUsers(BatchUserIdsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<UserBasicResponse>> BanUser(int userId)
        {
            var response = await _httpClient.PatchAsync($"/api/admin/User/{userId}/ban", null);

            return await Deserialize<Models.ApiResponse<UserBasicResponse>>(response)
                   ?? Models.ApiResponse<UserBasicResponse>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<DishResponse>> CreateDish(CreateDishRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<RecipeResponse>> CreateRecipe(CreateRecipeRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<UserResponseDTO>> CreateUser(CreateUserRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/admin/User/create", content);

            return await Deserialize<Models.ApiResponse<UserResponseDTO>>(response)
                   ?? Models.ApiResponse<UserResponseDTO>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task<Models.ApiResponse<BatchImportResult>> CreateUsersFromCsv(IFormFile file)
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("/api/admin/User/upload-csv", content);

            return await Deserialize<Models.ApiResponse<BatchImportResult>>(response)
                   ?? Models.ApiResponse<BatchImportResult>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<bool>> DeactiveBatchUsers(BatchUserIdsRequest request)
        {
            throw new NotImplementedException();
        }

        

        public Task<Models.ApiResponse<bool>> DeleteBatchUsers(BatchUserIdsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<bool>> DeleteDish(int dishId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<bool>> DeleteRecipe(int recipeId)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<UserResponseDTO>> DeleteUser(int userId)
        {
            var response = await _httpClient.DeleteAsync($"/api/admin/User/users/{userId}");

            return await Deserialize<Models.ApiResponse<UserResponseDTO>>(response)
                   ?? Models.ApiResponse<UserResponseDTO>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task<Models.ApiResponse<DashboardMetricResponse>> GetDashboardMetrics()
        {
            var response = await _httpClient.GetAsync("/api/admin/Dashboard/dashboard");

            return await Deserialize<Models.ApiResponse<DashboardMetricResponse>>(response)
                   ?? Models.ApiResponse<DashboardMetricResponse>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task<Models.ApiResponse<List<DishChartResponse>>> GetDishChartMonthly()
        {
            var response = await _httpClient.GetAsync("/api/admin/Dashboard/monthly-chart");

            return await Deserialize<Models.ApiResponse<List<DishChartResponse>>>(response)
                   ?? Models.ApiResponse<List<DishChartResponse>>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<DishDetailResponse>> GetDishDetail(int dishId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<Models.Response.PagedResponse<DishResponse>>> GetDishes(string? searchQuery, int? categoryId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<List<DishChartResponse>>> GetRecipeChartMonthly()
        {
            var response = await _httpClient.GetAsync("/api/admin/Dashboard/monthly-chart");

            return await Deserialize<Models.ApiResponse<List<DishChartResponse>>>(response)
                   ?? Models.ApiResponse<List<DishChartResponse>>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<RecipeDetailResponse>> GetRecipeDetail(int recipeId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<Models.Response.PagedResponse<RecipeResponse>>> GetRecipes(int? dishId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<UserDetailResponse>> GetUserDetail(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/admin/User/{userId}");

            return await Deserialize<Models.ApiResponse<UserDetailResponse>>(response)
                   ?? Models.ApiResponse<UserDetailResponse>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task< Models.ApiResponse<Models.Response.PagedResponse<UserResponseMgtDTO>>> GetUsers(string? searchQuery, UserStatus? status, int page, int pageSize)
        {
            var query = new List<string>
{
    $"page={page}",
    $"pageSize={pageSize}"
};

            if (!string.IsNullOrEmpty(searchQuery))
                query.Add($"searchQuery={searchQuery}");

            if (status.HasValue)
                
                Console.WriteLine($"Status value: {status.Value}"); // Debug log to check the value
            
            query.Add($"userStatus={status}");

            var url = "/api/admin/User?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);
            return await Deserialize<Models.ApiResponse<Models.Response.PagedResponse<UserResponseMgtDTO>>>(response)
                   ?? Models.ApiResponse<Models.Response.PagedResponse<UserResponseMgtDTO>>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task<Models.ApiResponse<bool>> RecoverUser(int userId)
        {
            var response = await _httpClient.PatchAsync($"/api/admin/User/{userId}/recover", null);

            return await Deserialize<Models.ApiResponse<bool>>(response)
                   ?? Models.ApiResponse<bool>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<bool>> RejectDish(int dishId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<DishResponse>> UpdateDish(int dishId, UpdateDishRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<RecipeResponse>> UpdateRecipe(int recipeId, UpdateRecipeRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<UserResponseDTO>> UpdateUser(int userId, UpdateUserRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/admin/User/update/{userId}", content);

            return await Deserialize<Models.ApiResponse<UserResponseDTO>>(response)
                   ?? Models.ApiResponse<UserResponseDTO>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public Task<Models.ApiResponse<string>> UploadImage(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<Models.ApiResponse<bool>> DeactiveUser(int userId)
        {
            throw new NotImplementedException();
        }

        Task<Models.ApiResponse<List<RecipeChartResponse>>> IAdminClient.GetRecipeChartMonthly()
        {
            throw new NotImplementedException();
        }

        public async Task<Models.ApiResponse<List<ChartCategoryDishResponse>>> GetCategoryChart()
        {
            var response = await _httpClient.GetAsync("/api/admin/Dashboard/category-chart");

            return await Deserialize<Models.ApiResponse<List<ChartCategoryDishResponse>>>(response)
                   ?? Models.ApiResponse<List<ChartCategoryDishResponse>>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }

        public async Task<Models.ApiResponse<UsersStatistic>> GetUserStatistics()
        {
            var response = await _httpClient.GetAsync("/api/admin/User/statistics");
            return await Deserialize<Models.ApiResponse<UsersStatistic>>(response)
                   ?? Models.ApiResponse<UsersStatistic>.Fail("ERROR", null, "SERVER_ERROR", 500);
        }
    }
}
