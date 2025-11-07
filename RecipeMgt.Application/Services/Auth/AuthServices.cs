using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeMgt.Application.DTOs.Request.Auth;
using RecipeMgt.Application.DTOs.Response.Auth;
using RecipeMgt.Application.Utils;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Users;
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

                    return new LoginResponse { Success = false, Message = "Wrong email or password" };
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

        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            try
            {
                var userName= registerRequest.UserName.Trim();
                var email= registerRequest.Email.Trim();
                var password= registerRequest.Password.Trim();

                var result = await checkDuplicateEmail(email);
                if (result)
                {
                    return new RegisterResponse
                    {
                        Success=false,
                        Message= "Email existed! Try again with another one"
                    };
                }

                var found= await _userRepository.getUserByUsername(userName);
                if(found!=null)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "userName existed! Try again with another one"
                    };
                }
                var payload = new User
                {
                    CreatedAt = DateTime.Now,
                    Email = email,
                    PasswordHash = UserUtils.HashPassword(password),
                    FullName = userName,
                    RoleId= 2
                    
                };
                var register = await _userRepository.createUser(payload);
                return new RegisterResponse { Success = true, Message = "Register successfully" };

                
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error during staff login for email {Email}", registerRequest.Email);
                return new RegisterResponse
                {
                    Success= false,
                    Message= "An error occurred during login"
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]??"");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];


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

        private async Task<bool> checkDuplicateEmail(string email)
        {
            return await _userRepository.checkDuplicateEmail(email);
        }
    }
}
