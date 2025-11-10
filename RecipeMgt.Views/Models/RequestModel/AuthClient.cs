using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Models.RequestModel
{
    public class AuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string _jwtToken;

        public AuthClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }
        public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
        {
            var payload = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true && !string.IsNullOrEmpty(result.Data?.Token))
            {
                _jwtToken = result.Data.Token;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }

            return result;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(string email, string password, string username)
        {
            var payload = new { email, password, username };
            Console.WriteLine(payload.username);
           
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/register", content);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<RegisterResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ApiResponse<ChangePasswordResponse>> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var payload = new { email, oldPassword, newPassword };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/api/auth/change-password")
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(_jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            }

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<ChangePasswordResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public string GetToken() => _jwtToken;
    }
}

