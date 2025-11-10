using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Net.Http.Headers;

namespace RecipeMgt.Views.Models.RequestModel
{
    public class DishClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public DishClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<List<DishResponse>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/dish");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DishResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<DishResponse>();
        }

        public async Task<List<DishResponse>> GetByCategoryAsync(int categoryId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/dish/cate/{categoryId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DishResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<DishResponse>();
        }

        public async Task<DishDetailResponse?> GetDetailAsync(int dishId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/dish/{dishId}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DishDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<CreateDishResponse> CreateAsync(MultipartFormDataContent form)
        {
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/dish/create", form);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<UpdateDishResponse> UpdateAsync(MultipartFormDataContent form)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/api/dish/update")
            {
                Content = form
            };
            var resp = await _httpClient.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UpdateDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<DeleteDishResponse> DeleteAsync(int dishId)
        {
            var resp = await _httpClient.DeleteAsync($"{_baseUrl}/api/dish/delete/{dishId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
