using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NuGet.Common;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {

        private readonly IAuthClient _authClient;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthClient authClient, ILogger<AuthController> logger)
        {
            _authClient = authClient;
            _logger = logger;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet("LoginGoogle")]
        public IActionResult LoginGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
            };
            _logger.LogInformation("Runnnnnn into hererrrrrrrrrr");
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);

        }

        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            _logger.LogInformation($"{nameof(GoogleResponse)}");
            var result = await HttpContext.AuthenticateAsync("Cookies");
            _logger.LogInformation($"{result}");
            if (result == null || !result.Succeeded)
            {
                _logger.LogError(result?.Failure?.InnerException?.Message ?? "ERROR WHEN AUTHENTICATED");
                return RedirectToAction("Login");
            }

            foreach (var token in result.Properties.GetTokens())
            {
                _logger.LogInformation($"{token.Name}: {token.Value}");
            }

            var email = result.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = result.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            _logger.LogInformation($"Email: {email}");
            _logger.LogInformation($"Name: {name}");
            

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                _logger.LogError("Unable to get email");
                return RedirectToAction("Login");
            }

            var apiResponse = await _authClient.LoginWithGoogleAsync(email, name);

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data?.Token == null)
            {
                return RedirectToAction("Login");
            }

            var jwt = apiResponse.Data.Token;

            await HttpContext.SignOutAsync("Cookies");

            HttpContext.Session.SetString("JwtToken", jwt);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost("Login", Name = "AuthLogin")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _authClient.LoginAsync(email, password);

            if (result?.Success == true && result.Data?.Token != null)
            {
               
                HttpContext.Session.SetString("JwtToken", result.Data?.Token?? "");
                HttpContext.Session.SetString("UserName", result.Data?.User?.FullName?? "");
                HttpContext.Session.SetString("UserEmail", result.Data?.User?.Email?? "");

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(result.Data?.Token);

                var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                // ✅ redirect theo role
                if (role == "Admin")
                    return RedirectToAction("Dashboard", "Management", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result?.Message == "BAD_CREDENTIALS")
                {
                    ViewBag.Error = "Email or password is incorrect";
                }
                else if (result?.Message == "Validation failed")
                {
                    ViewBag.Error = string.Join(", ", result?.Errors ?? []);
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Login failed";
                }

                return View();
            }
        }

        [HttpPost("Register", Name = "AuthRegister")]

        public async Task<IActionResult> Register(string email, string password, string username)
        {
            var result= await _authClient.RegisterAsync(email, password, username);
            if (result.Success)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập lại";
                return Redirect("/Auth/Login");

            }
            else
            {
                ViewBag.Error = result.Message;
                return View();
            }
        }

        [HttpPost]

        public async Task<IActionResult> ChangePassword(string email, string currentPassword, string newPassword)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            var result = await _authClient.ChangePasswordAsync(email, currentPassword,newPassword, token);
            if (result.Success)
            {
                TempData["ChangePasswordSuccess"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = result.Message;
                return View();
            }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
