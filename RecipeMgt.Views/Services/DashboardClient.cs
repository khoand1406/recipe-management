using RecipeMgt.Views.Common.Constant;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class DashboardClient: IDashboardClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<DashboardClient> _logger;
        public DashboardClient(HttpClient httpClient, ILogger<DashboardClient> logger)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        private async Task<ApiResponse<List<T>>> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            _logger.LogInformation("Request to {Endpoint} returned status code {StatusCode}", endpoint, response.StatusCode);
            _logger.LogDebug("Response content: {Content}", await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"API Error: {error}");
            }

            var stream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<ApiResponse<List<T>>>(stream, _jsonOptions);

            return result?? new ApiResponse<List<T>> { Success = false, Message = "Failed to deserialize response." };
        }
        public Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync() => GetAsync<CategoryDto>(Endpoints.ApiCategoryEndpoint);
        

        public Task<ApiResponse<List<DishResponse>>> GetDishResponsesAsync()=> GetAsync<DishResponse>(Endpoints.ApiDishEndpoint);


        public Task<ApiResponse<List<DishResponse>>> GetTopDishViewAsync()=> GetAsync<DishResponse>(Endpoints.ApiTopDishViewEndpoint);

        public Task<ApiResponse<List<DishResponse>>> GetLatestDishesAsync()
            => GetAsync<DishResponse>(Endpoints.ApiLatestDishesEndpoint);

        public Task<ApiResponse<List<UserResponse>>> GetTopContributorAsync()
            => GetAsync<UserResponse>(Endpoints.ApiTopContributorEndpoint);
    }
}