using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Auth;
using RecipeMgt.Application.DTOs.Response.Auth;
using RecipeMgt.Application.Services.Auth;
using System.ComponentModel.DataAnnotations;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private ILogger<AuthController> _logger;
        private readonly IValidator<Application.DTOs.Request.Auth.LoginRequest> _loginValidator;
        private readonly IValidator<Application.DTOs.Request.Auth.RegisterRequest> _registerValidator;
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
        public async Task<ActionResult<ApiResponse<LoginResponse>>> loginUser([FromBody] Application.DTOs.Request.Auth.LoginRequest loginRequest)
        {
            try
            {
                var validation= await _loginValidator.ValidateAsync(loginRequest);
                if(!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validation.Errors
                                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                            .ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                var result = await _authServices.Login(loginRequest);

                if(result.Success)
                {
                    _logger.LogInformation("Staff login successful for email: {Email}", loginRequest.email);
                    return Ok(new ApiResponse<LoginResponse>
                    {
                        Success = true,
                        Data = result,
                        Message = "Login successful",
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                else
                {
                    return Unauthorized(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = result.Message,
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during staff login for email: {Email}", loginRequest.email);
                return StatusCode(500, new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "An error occurred during login",
                    RequestId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] Application.DTOs.Request.Auth.RegisterRequest request)
        {
            try
            {
                var validation= await _registerValidator.ValidateAsync(request);
                if(!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<RegisterResponse>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validation.Errors
                                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                            .ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                
                var result= await _authServices.Register(request);
                if (result.Success)
                {
                    return Ok(new ApiResponse<Application.DTOs.Request.Auth.RegisterRequest>
                    {
                        Success= true,
                        Message= result.Message,
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<Application.DTOs.Request.Auth.RegisterRequest>
                    {
                        Success = false,
                        Message = result.Message,
                        RequestId = HttpContext.TraceIdentifier
                    });
                }

            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user register with email: " + request.Email);
                return StatusCode(500, new ApiResponse<RegisterResponse> { Success = false, Message= ex.Message, RequestId= HttpContext.TraceIdentifier });
            }
        }

        [HttpPatch("change-password")]

        public async Task<ActionResult<ApiResponse<ChangePasswordResponse>>> changePassword([FromBody]ChangePasswordRequest request)
        {
            try
            {
                var validation = await _changePasswordValidator.ValidateAsync(request);
                if (!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<ChangePasswordResponse>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validation.Errors
                                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                            .ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                

                var result = await _authServices.changePassword(request);
                if (result.Success)
                {
                    return new ApiResponse<ChangePasswordResponse>
                    {
                        Success = true,
                        Message = result.Message,
                    };
                }
                else
                {
                    return BadRequest(new ApiResponse<ChangePasswordResponse>
                    {
                        Success = false,
                        Message = result.Message,
                        RequestId = HttpContext.TraceIdentifier

                    });
                }

            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user register with email: " + request.Email);
                return StatusCode(500, new ApiResponse<RegisterResponse> { Success = false, Message = ex.Message, RequestId = HttpContext.TraceIdentifier });
            }
        }
    }
}
