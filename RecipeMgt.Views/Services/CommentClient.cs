using RecipeMgt.Views.Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using RecipeMgt.Views.Interface;

namespace RecipeMgt.Views.Services
{
    public class CommentClient:ICommentClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public CommentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public void SetBearerToken(string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<CommentResponseDTO>> GetCommentsByRecipeIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"/api/recipe/{recipeId}/comments");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CommentResponseDTO>>(json, _options)
                   ?? new List<CommentResponseDTO>();
        }

        public async Task<AddCommentResponse> AddCommentAsync(int recipeId, string content)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(content);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync($"/api/recipe/{recipeId}/comment", httpContent);
                
                if (resp.IsSuccessStatusCode)
                {
                    return new AddCommentResponse { Success = true, Message = "Comment added successfully" };
                }
                
                return new AddCommentResponse { Success = false, Message = "Failed to add comment" };
            }
            catch (Exception ex)
            {
                return new AddCommentResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
