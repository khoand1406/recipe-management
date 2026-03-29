using Azure;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Views.Common.Constant;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class DishClient : IDishClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public DishClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Models.Response.DishResponse>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync(Endpoints.ApiDishEndpoint);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Models.Response.DishResponse>>(json, _jsonOptions)
                   ?? [];
        }

        public async Task<PagedResponse<CategoryDishResponse>> GetDishesAsync(
    int page,
    int pageSize,
    string? searchQuery,
    int? categoryId)
        {
            var url = $"/api/dish?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(searchQuery))
                url += $"&searchQuery={searchQuery}";

            if (categoryId.HasValue)
                url += $"&categoryId={categoryId}";

            var resp = await _httpClient.GetAsync(url);

            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();

            var result= JsonSerializer.Deserialize<ApiResponse<PagedResponse<CategoryDishResponse>>>(
                json,
                _jsonOptions
            )!;
            return result.Data!;
        }

        public async Task<ApiResponse<Models.Response.DishDetailResponse?>> GetDetailAsync(int dishId)
        {
            var resp = await _httpClient.GetAsync(Endpoints.ApiDishDetailEndpoint);
            if (!resp.IsSuccessStatusCode) return ApiResponse<Models.Response.DishDetailResponse?>.Fail("Error fetch detail data", null, "INTERNAL_SERVER_ERROR", 500);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<Models.Response.DishDetailResponse?>>(json, _jsonOptions)!;
        }

        public async Task<Models.Response.CreateDishResponse> CreateAsync(MultipartFormDataContent form)
        {
            var resp = await _httpClient.PostAsync(Endpoints.ApiDishCreateEndpoint, form);
            
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new ApplicationException($"API Error: {error}");
            }
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Models.Response.CreateDishResponse>(json, _jsonOptions);
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
            return JsonSerializer.Deserialize<UpdateDishResponse>(json, _jsonOptions)!;
        }

        public async Task<DeleteDishResponse> DeleteAsync(int dishId)
        {
            var resp = await _httpClient.DeleteAsync(Endpoints.ApiDishDeleteEndpoint);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteDishResponse>(json, _jsonOptions)!;
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
