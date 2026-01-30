using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs.Request.Auth;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Auth;
using RecipeMgt.Application.Exceptions;
using RecipeMgt.Application.Utils;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.RefreshTokens;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<AuthServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IOAuthService _ioAuthService;


        public AuthServices(IUserRepository userRepository, ILogger<AuthServices> logger, IConfiguration configuration, IRefreshTokenRepository refreshToken, IJwtService jwtService, IOAuthService ioAuthService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
            _refreshTokenRepository = refreshToken;
            _jwtService = jwtService;
            _ioAuthService = ioAuthService;
        }

        public Task<ChangePasswordResponse> changePassword(ChangePasswordRequest changePasswordRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var email = request.email.Trim();
            var password = request.password.Trim();

            var user = await _userRepository.getUserByEmail(email)
                ?? throw new AuthenticationException(AuthenticationError.AuthenError);

            if (!UserUtils.VerifyPassword(password, user.PasswordHash))
                throw new AuthenticationException(AuthenticationError.BadCredentials);

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await _refreshTokenRepository.RevokeAllByUserAsync(user.UserId);
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            return new LoginResponse
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                User = new UserResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt ?? DateTime.UtcNow
                },
                Message = "Login successful"
            };
        }


        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var email = request.Email.Trim();
            var username = request.UserName.Trim();
            var password = request.Password.Trim();

            if (await _userRepository.checkDuplicateEmail(email))
                throw new AuthenticationException(AuthenticationError.DuplicateEmail);

            if (await _userRepository.getUserByUsername(username) != null)
                throw new AuthenticationException(AuthenticationError.DuplicateUsername);

            var user = new User
            {
                Email = email,
                FullName = username,
                PasswordHash = UserUtils.HashPassword(password),
                RoleId = RoleConstants.USER_ROLE_ID,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.createUser(user);

            return new RegisterResponse
            {
                Success = true,
                Message = "Register successfully"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("role", "user")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<TokenResponse> Refreshtoken(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken) ?? throw new AuthenticationException(AuthenticationError.InvalidToken);
            if (storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow) throw new AuthenticationException(AuthenticationError.TokenExpired);
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            var user = storedToken.User;
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            await _refreshTokenRepository.UpdateAsync(storedToken);
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });
            return new TokenResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };


        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(string idToken)
        {
            var payload = await _ioAuthService.VerifyAsync(idToken);

            var user = await _userRepository.UpsertGoogleUserAsync(
                payload.Subject,
                payload.Email,
                payload.Name,
                payload.Picture
            );

            return new AuthResponse
            {
                AccessToken = _jwtService.GenerateJWTToken(user),
                ExpiredAt = DateTime.UtcNow.AddMinutes(30)
            };
        }
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
