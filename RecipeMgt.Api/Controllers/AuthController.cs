using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Auth;
using RecipeMgt.Application.DTOs.Response.Auth;
using RecipeMgt.Application.Services.Auth;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private ILogger<AuthController> _logger;
        private readonly IValidator<Application.DTOs.Request.Auth.LoginRequest> _loginValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;

        public AuthController(
           IAuthServices authServices,
           ILogger<AuthController> logger,
           IValidator<Application.DTOs.Request.Auth.LoginRequest> loginValidator,
           IValidator<Application.DTOs.Request.Auth.RegisterRequest> registerValidator,
           IValidator<ChangePasswordRequest> changePasswordValidator)
        {
            _authServices = authServices;
            _logger = logger;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _changePasswordValidator = changePasswordValidator;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
            [FromBody] LoginRequest request)
        {
            var validation = await _loginValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(ApiResponseFactory.Fail(validation, HttpContext));

            var result = await _authServices.Login(request);

            return Ok(ApiResponseFactory.Success(result, HttpContext));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
        {
            var validation = await _registerValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return BadRequest(ApiResponseFactory.Fail(validation, HttpContext));
            }

            var result = await _authServices.Register(request);
            return Ok(ApiResponseFactory.Success(result, HttpContext));
        }
        [Authorize]
        [HttpPatch("change-password")]

        public async Task<ActionResult<ApiResponse<ChangePasswordResponse>>> changePassword([FromBody] ChangePasswordRequest request)
        {
                var validation = await _changePasswordValidator.ValidateAsync(request);
                if (!validation.IsValid)
                {
                    return BadRequest(ApiResponseFactory.Fail(validation, HttpContext)) ;
                }
                var result = await _authServices.changePassword(request);
                return Ok(ApiResponseFactory.Success(result, HttpContext)); 
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _authServices.Refreshtoken(request.RefreshToken);
            return Ok(ApiResponseFactory.Success(result, HttpContext));
        }

    }
}
