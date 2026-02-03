using RecipeMgt.Application.DTOs.Request.Auth;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public interface IAuthServices
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);

        Task<ChangePasswordResponse> changePassword(ChangePasswordRequest changePasswordRequest); 

        Task<RegisterResponse> Register(RegisterRequest registerRequest);

        Task<TokenResponse> Refreshtoken(string refreshToken);

        Task<AuthResponse> LoginWithGoogleAsync(string idToken);
    }
}
