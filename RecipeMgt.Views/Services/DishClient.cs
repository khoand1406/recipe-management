using Azure;
using RecipeMgt.Views.Common.Constant;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class DishClient : IDishClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public DishClient(HttpClient httpClient, JsonSerializerOptions options)
        {
            _httpClient = httpClient;
            _jsonOptions = options;
        }

        public async Task<List<DishResponse>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync(Endpoints.ApiDishEndpoint);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DishResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<DishResponse>();
        }

        public async Task<List<DishResponse>> GetByCategoryAsync(int categoryId)
        {
            var resp = await _httpClient.GetAsync(Endpoints.ApiDishByCategoryEndpoint);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DishResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<DishResponse>();
        }

        public async Task<DishDetailResponse?> GetDetailAsync(int dishId)
        {
            var resp = await _httpClient.GetAsync(Endpoints.ApiDishDetailEndpoint);
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DishDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<CreateDishResponse> CreateAsync(MultipartFormDataContent form)
        {
            var resp = await _httpClient.PostAsync(Endpoints.ApiDishCreateEndpoint, form);
            
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new ApplicationException($"API Error: {error}");
            }
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CreateDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result == null)
            {
                throw new ApplicationException($"API Error: {result?.Message ?? "Unknown error"}");
            }
            return result;
        }

        public async Task<UpdateDishResponse> UpdateAsync(MultipartFormDataContent form)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, Endpoints.ApiDishUpdateEndpoint)
            {
                Content = form
            };
            var resp = await _httpClient.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UpdateDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<DeleteDishResponse> DeleteAsync(int dishId)
        {
            var resp = await _httpClient.DeleteAsync(Endpoints.ApiDishDeleteEndpoint);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteDishResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        Task IDishClient.DeleteAsync(int id)
        {
            return DeleteAsync(id);
        }

        public Task UpdateAsync(int id, string dishName, string description, int categoryId, List<IFormFile>? images)
        {
            throw new NotImplementedException();
        }
    }
}
