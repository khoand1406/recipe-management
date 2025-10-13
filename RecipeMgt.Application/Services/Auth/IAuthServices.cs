using RecipeMgt.Application.DTOs.Request;
using RecipeMgt.Application.DTOs.Response;
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
    }
}
