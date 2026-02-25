using RecipeMgt.Views.Models.Response;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Common.Constant;

namespace RecipeMgt.Views.Services
{
    public class RecipeClient: IRecipeClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        public RecipeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

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

        public async Task<ApiResponse<RecipeResponse>> GetByIdAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"/api/recipe/{recipeId}");
            if (!resp.IsSuccessStatusCode) return ApiResponse<RecipeResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<RecipeResponse>>(json, _options)?? ApiResponse<RecipeResponse>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<List<RecipeResponse>>> GetByDishAsync(int dishId)
        {
            var resp = await _httpClient.GetAsync($"/api/recipe/dish/{dishId}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<RecipeResponse>>>(json, _options);
            return result ?? ApiResponse<List<RecipeResponse>>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<List<RecipeWithUserInfo>>> GetMineAsync()
        {
            var resp = await _httpClient.GetAsync($"/api/recipe");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<RecipeWithUserInfo>>>(json,_options)
                   ?? ApiResponse<List<RecipeWithUserInfo>>.Fail("INTERAL SERVER ERROR", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR); 
        }

        public async Task<ApiResponse<List<RecipeResponse>>> GetByFilterAsync()
        {
            var resp = await _httpClient.GetAsync($"/api/recipe/by-filter");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<RecipeResponse>>>(json, _options)!;
        }

        public async Task<ApiResponse<PagedResponse<RecipeResponse>>> SearchAsync(object query)
        {
            var queryString = ToQueryString(query);
            var resp = await _httpClient.GetAsync($"/api/recipe/search{queryString}");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<PagedResponse<RecipeResponse>>>(json, _options)!;
        }

        public async Task<ApiResponse<UploadImageResult>> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "file", fileName);

            var resp = await _httpClient.PostAsync($"/api/recipe/upload-image", form);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UploadImageResult>>(json, _options)!;
        }

        public async Task<ApiResponse<CreateRecipeResponse>> CreateAsync(MultipartFormDataContent form)
        {
            var resp = await _httpClient.PostAsync($"/api/recipe/create", form);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<CreateRecipeResponse>>(json, _options)!;
        }

        public async Task<ApiResponse<UpdateRecipeResponse>> UpdateAsync(MultipartFormDataContent form)
        {
            var req = new HttpRequestMessage(HttpMethod.Put, $"/api/recipe/update")
            {
                Content = form
            };
            var resp = await _httpClient.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UpdateRecipeResponse>>(json, _options)!;
        }

        public async Task<ApiResponse<DeleteRecipeResponse>> DeleteAsync(int recipeId)
        {
            var resp = await _httpClient.DeleteAsync($"/api/recipe/delete/{recipeId}");
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<DeleteRecipeResponse>>(json, _options)!;
        }

        public async Task<bool> AddCommentAsync(int recipeId, string content)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"/api/recipe/{recipeId}/comment", stringContent);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> AddBookmarkAsync(int recipeId)
        {
            var resp = await _httpClient.PostAsync($"/api/recipe/{recipeId}/bookmark", null);
            return resp.IsSuccessStatusCode;
        }

        public async Task<ApiResponse<List<CommentResponseDTO>>> GetCommentsAsync(int recipeId)
        {
            var resp = await _httpClient.GetAsync($"/api/recipe/{recipeId}/comments");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<List<CommentResponseDTO>>>(json, _options) ??
                    ApiResponse<List<CommentResponseDTO>>.Fail("Invalid server response", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
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
            var response = await _httpClient.GetAsync($"/api/category");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(json,_options);

            return categories ?? [];
        }

       
    }
}
