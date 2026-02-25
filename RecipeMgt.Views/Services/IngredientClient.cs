using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Text;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Common.Constant;

namespace RecipeMgt.Views.Services
{
    public class IngredientClient:IIngredientClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions options;

        public IngredientClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<ApiResponse<IngredientResponse>> GetByIdAsync(int ingredientId)
        {
            var resp = await _httpClient.GetAsync($"/api/ingredient/{ingredientId}");
            if (!resp.IsSuccessStatusCode) return ApiResponse<IngredientResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<IngredientResponse>>(json, options)?? ApiResponse<IngredientResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<List<IngredientResponse>>> GetByRecipeIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"/api/ingredient/recipe/{recipeId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<IngredientResponse>>>(json, options)!;
                  
        }

        public async Task<ApiResponse<CreateIngredientResponse>> CreateAsync(int recipeId, string name, string quantity)
        {
            var payload = new { RecipeId = recipeId, Name = name, Quantity = quantity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"/api/ingredient/create", content);
            if (!resp.IsSuccessStatusCode) return ApiResponse<CreateIngredientResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<CreateIngredientResponse>>(json, options )!;
        }

        public async Task<ApiResponse<UpdateIngredientResponse>> UpdateAsync(int ingredientId, string name, string quantity)
        {
            var payload = new { IngredientId = ingredientId, Name = name, Quantity = quantity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/ingredient/update", content);
            if (!resp.IsSuccessStatusCode) return ApiResponse<UpdateIngredientResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UpdateIngredientResponse>>(json, options)!;
        }

        public async Task<ApiResponse<DeleteIngredientResponse>> DeleteAsync(int ingredientId)
        {
            var resp = await _httpClient.DeleteAsync($"/api/ingredient/delete/{ingredientId}");
            if(!resp.IsSuccessStatusCode) return ApiResponse<DeleteIngredientResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<DeleteIngredientResponse>>(json, options)!;
        }
    }
}
