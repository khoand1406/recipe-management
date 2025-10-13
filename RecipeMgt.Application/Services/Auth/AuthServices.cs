using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeMgt.Application.DTOs.Request;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.Utils;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _activeTokens = new();

        public AuthServices(IUserRepository userRepository, ILogger<AuthServices> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ChangePasswordResponse> changePassword(ChangePasswordRequest changePasswordRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            try
            {
                var inputEmail = loginRequest.email.Trim();
                var inputPassword = loginRequest.password.Trim();
                var user = await _userRepository.getUserByEmail(inputEmail);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: user not found or inactive. Email: {Email}", inputEmail);

                    return new LoginResponse { Success = false, Message = "User not found" };
                }
                if(!UserUtils.VerifyPassword(inputPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: password mismatch. Email: {Email}, InputPassword: '{InputPassword}', DbPassword: '{DbPassword}'", inputEmail, inputPassword, user.PasswordHash);

                    return new LoginResponse { Success = false, Message = "Invalid password" };
                }
                var token= GenerateJwtToken(user);

                _activeTokens[token] = user.Email;

                return new LoginResponse
                {
                    Success = true,
                    User = new UserResponse
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt ?? DateTime.Now

                    },
                    Token = token,
                    ExpiresAt = DateTime.Now.AddHours(8),
                    Message = "Login Successful"


                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during staff login for email {Email}", loginRequest.email);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
            
        }

        Task<RegisterResponse> IAuthServices.Register(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "ChapTatCaTheLoaiBug");
            var issuer = _configuration["Jwt:Issuer"] ?? "FastRailSystem";
            var audience = _configuration["Jwt:Audience"] ?? "FastRailStaff";


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", "user")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
