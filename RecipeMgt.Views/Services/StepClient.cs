using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Text;

namespace RecipeMgt.Views.Services
{
    public class StepClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public StepClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<StepResponse?> GetByIdAsync(int stepId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/step/{stepId}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StepResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<StepResponse>> GetByRecipeIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/step/recipe/{recipeId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<StepResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<StepResponse>();
        }

        public async Task<CreateStepResponse> CreateAsync(int recipeId, int stepNumber, string instruction)
        {
            var payload = new { RecipeId = recipeId, StepNumber = stepNumber, Instruction = instruction };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/step/create", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateStepResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<UpdateStepResponse> UpdateAsync(int stepId, int stepNumber, string instruction)
        {
            var payload = new { StepId = stepId, StepNumber = stepNumber, Instruction = instruction };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{_baseUrl}/api/step/update", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UpdateStepResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<DeleteStepResponse> DeleteAsync(int stepId)
        {
            var resp = await _httpClient.DeleteAsync($"{_baseUrl}/api/step/delete/{stepId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteStepResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
