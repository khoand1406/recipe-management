using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.Services.Auth;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private ILogger<AuthController> _logger;

        public AuthController(IAuthServices authServices, ILogger<AuthController> logger)
        {
            _authServices = authServices;
            _logger = logger;
        }

        [HttpPost("/login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> loginUser([FromBody] Application.DTOs.Request.LoginRequest loginRequest)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(loginRequest.email) || string.IsNullOrWhiteSpace(loginRequest.password))
                {
                    _logger.LogError("Email or password can not be empty");

                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Email and password are required",
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
    }
}
