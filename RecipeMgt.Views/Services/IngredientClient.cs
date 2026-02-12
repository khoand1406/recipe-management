using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Text;

namespace RecipeMgt.Views.Services
{
    public class IngredientClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public IngredientClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<IngredientResponse?> GetByIdAsync(int ingredientId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/ingredient/{ingredientId}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IngredientResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<IngredientResponse>> GetByRecipeIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/ingredient/recipe/{recipeId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IngredientResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<IngredientResponse>();
        }

        public async Task<CreateIngredientResponse> CreateAsync(int recipeId, string name, string quantity)
        {
            var payload = new { RecipeId = recipeId, Name = name, Quantity = quantity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/ingredient/create", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateIngredientResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<UpdateIngredientResponse> UpdateAsync(int ingredientId, string name, string quantity)
        {
            var payload = new { IngredientId = ingredientId, Name = name, Quantity = quantity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{_baseUrl}/api/ingredient/update", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UpdateIngredientResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<DeleteIngredientResponse> DeleteAsync(int ingredientId)
        {
            var resp = await _httpClient.DeleteAsync($"{_baseUrl}/api/ingredient/delete/{ingredientId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteIngredientResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
