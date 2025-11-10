using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace RecipeMgt.Views.Models.RequestModel
{
    public class RecipeClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public RecipeClient(string baseUrl)
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

        public async Task<RecipeResponse?> GetByIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/{recipeId}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecipeResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<RecipeResponse>> GetByDishAsync(int dishId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/dish/{dishId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecipeResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<RecipeResponse>();
        }

        public async Task<List<RecipeWithUserInfo>> GetMineAsync()
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecipeWithUserInfo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<RecipeWithUserInfo>();
        }

        public async Task<ApiResponse<IEnumerable<RecipeResponse>>> GetByFilterAsync()
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/by-filter");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<IEnumerable<RecipeResponse>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<ApiResponse<PagedResponse<RecipeResponse>>> SearchAsync(object query)
        {
            var queryString = ToQueryString(query);
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/search{queryString}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<PagedResponse<RecipeResponse>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<UploadImageResult> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "file", fileName);

            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/recipe/upload-image", form);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UploadImageResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<ApiResponse<CreateRecipeResponse>> CreateAsync(MultipartFormDataContent form)
        {
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/recipe/create", form);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<CreateRecipeResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<ApiResponse<UpdateRecipeResponse>> UpdateAsync(MultipartFormDataContent form)
        {
            var req = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl}/api/recipe/update")
            {
                Content = form
            };
            var resp = await _httpClient.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UpdateRecipeResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<ApiResponse<DeleteRecipeResponse>> DeleteAsync(int recipeId)
        {
            var resp = await _httpClient.DeleteAsync($"{_baseUrl}/api/recipe/delete/{recipeId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<DeleteRecipeResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<bool> AddCommentAsync(int recipeId, string content)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/recipe/{recipeId}/comment", stringContent);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> AddBookmarkAsync(int recipeId)
        {
            var resp = await _httpClient.PostAsync($"{_baseUrl}/api/recipe/{recipeId}/bookmark", null);
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<CommentResposneDTO>> GetCommentsAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"{_baseUrl}/api/recipe/{recipeId}/comments");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CommentResposneDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<CommentResposneDTO>();
        }

        private static string ToQueryString(object obj)
        {
            if (obj == null) return string.Empty;
            var props = from p in obj.GetType().GetProperties()
                        let v = p.GetValue(obj, null)
                        where v != null && !string.IsNullOrWhiteSpace(v.ToString())
                        select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(v!.ToString()!)}";
            var qs = string.Join("&", props);
            return string.IsNullOrEmpty(qs) ? string.Empty : $"?{qs}";
        }

        // GET api/category
        public async Task<List<CategoryDto>> GetRecipe()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/category");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return categories ?? new List<CategoryDto>();
        }
    }
}
