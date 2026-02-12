using RecipeMgt.Views.Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RecipeMgt.Views.Services
{
    public class CommentClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CommentClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
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
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/{recipeId}/comments");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CommentResponseDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<CommentResponseDTO>();
        }

        public async Task<AddCommentResponse> AddCommentAsync(int recipeId, string content)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(content);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync($"{_baseUrl}/api/recipe/{recipeId}/comment", httpContent);
                
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
