using RecipeMgt.Views.Common.Constant;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models.Response;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class DashboardClient: IDashboardClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public DashboardClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<List<T>> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"API Error: {error}");
            }

            var stream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);

            return result ?? [];
        }
        public Task<List<CategoryDto>> GetCategoriesAsync() => GetAsync<CategoryDto>(Endpoints.ApiCategoryEndpoint);
        

        public Task<List<DishResponse>> GetDishResponsesAsync()=> GetAsync<DishResponse>(Endpoints.ApiDishEndpoint);


        public Task<List<DishResponse>> GetTopDishViewAsync()=> GetAsync<DishResponse>(Endpoints.ApiTopDishViewEndpoint);

        public Task<List<DishResponse>> GetLatestDishesAsync()
            => GetAsync<DishResponse>(Endpoints.ApiLatestDishesEndpoint);

        public Task<List<UserResponse>> GetTopContributorAsync()
            => GetAsync<UserResponse>(Endpoints.ApiTopContributorEndpoint);
    }
}