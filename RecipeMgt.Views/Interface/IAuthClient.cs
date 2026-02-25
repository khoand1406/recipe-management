using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IAuthClient
    {
        public Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password);

        public Task<ApiResponse<RegisterResponse>> RegisterAsync(string email, string password, string username);

        public Task<ApiResponse<ChangePasswordResponse>> ChangePasswordAsync(string email, string oldPassword, string newPassword, string jwtToken);     
    }
}
