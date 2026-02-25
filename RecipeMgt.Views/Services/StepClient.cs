using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Text;
using RecipeMgt.Views.Interface;

namespace RecipeMgt.Views.Services
{
    public class StepClient:IStepClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public StepClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        }

        public async Task<StepResponse?> GetByIdAsync(int stepId)
        {
            var resp = await _httpClient.GetAsync($"/api/step/{stepId}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StepResponse>(json, _options);
        }

        public async Task<List<StepResponse>> GetByRecipeIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"/api/step/recipe/{recipeId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<StepResponse>>(json, _options)
                   ?? new List<StepResponse>();
        }

        public async Task<CreateStepResponse> CreateAsync(int recipeId, int stepNumber, string instruction)
        {
            var payload = new { RecipeId = recipeId, StepNumber = stepNumber, Instruction = instruction };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"/api/step/create", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CreateStepResponse>(json, _options)!;
        }

        public async Task<UpdateStepResponse> UpdateAsync(int stepId, int stepNumber, string instruction)
        {
            var payload = new { StepId = stepId, StepNumber = stepNumber, Instruction = instruction };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/step/update", content);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UpdateStepResponse>(json, _options)!;
        }

        public async Task<DeleteStepResponse> DeleteAsync(int stepId)
        {
            var resp = await _httpClient.DeleteAsync($"/api/step/delete/{stepId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeleteStepResponse>(json,_options)!;
        }
    }
}
