using RecipeMgt.Views.Models.Response;
using System.Text.Json;

namespace RecipeMgt.Views.Models.RequestModel
{
    public class DashboardClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public DashboardClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/category");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return categories ?? new List<CategoryDto>();
        }

    }
}
