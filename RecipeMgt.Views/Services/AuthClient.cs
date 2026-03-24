using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using RecipeMgt.Views.Models.Response;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Common.Constant;

namespace RecipeMgt.Views.Services
{
    public class AuthClient: IAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        

        public AuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
        {
            var payload = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(Endpoints.AuthLoginEndpoint, content);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json, _options)
                   ?? ApiResponse<LoginResponse>.Fail("Invalid server response", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<LoginResponse>> LoginWithGoogleAsync(string email, string name)
        {
            var payload= new { email, name };
            var content= new StringContent(JsonSerializer.Serialize(payload),Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Endpoints.AuthGoogleLogin, content);
            var json= await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json, _options) ?? ApiResponse<LoginResponse>.Fail("Invalid server response", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR); ;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(string email, string password, string username)
        {
            var payload = new { email, password, username };
            Console.WriteLine(payload.username);
           
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(Endpoints.AuthRegisterEndpoint, content);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<RegisterResponse>>(json, _options)?? ApiResponse<RegisterResponse>.Fail("Internal server response", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        public async Task<ApiResponse<ChangePasswordResponse>> ChangePasswordAsync(string email, string oldPassword, string newPassword, string jwtToken)
        {
            var payload = new { email, oldPassword, newPassword };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch,Endpoints.AuthUserChangePasswordEndpoint)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<ChangePasswordResponse>>(json, _options)?? ApiResponse<ChangePasswordResponse>.Fail("Internal Server Reponse", null, "SERVER_ERROR", (int?)StatusCode.INTERNAL_SERVER_ERROR);
        }

        
    }
}

