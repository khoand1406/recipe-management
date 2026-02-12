using RecipeMgt.Views.Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RecipeMgt.Views.Services
{
    public class BookmarkClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public BookmarkClient(string baseUrl)
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

        public async Task<List<BookmarkResponseDto>> GetMyBookmarksAsync()
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/bookmark/my-bookmarks");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BookmarkResponseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<BookmarkResponseDto>();
        }

        public async Task<AddBookmarkResponse> AddBookmarkAsync(int recipeId)
        {
            try
            {
                var resp = await _httpClient.PostAsync($"{_baseUrl}/api/bookmark/{recipeId}", null);
                
                if (resp.IsSuccessStatusCode)
                {
                    return new AddBookmarkResponse { Success = true, Message = "Bookmark added successfully" };
                }
                
                var errorJson = await resp.Content.ReadAsStringAsync();
                return new AddBookmarkResponse { Success = false, Message = "Failed to add bookmark" };
            }
            catch (Exception ex)
            {
                return new AddBookmarkResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
