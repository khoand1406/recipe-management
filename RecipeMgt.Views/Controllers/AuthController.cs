using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Models.RequestModel;
using Microsoft.Extensions.Configuration;

namespace RecipeMgt.Views.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthClient _authClient;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            _authClient = new AuthClient(apiBaseUrl);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _authClient.LoginAsync(email, password);

            if (result?.Success == true && result.Data?.Token != null)
            {
               
                Console.WriteLine(result.Data.Token);
                Console.WriteLine(result.Data?.User?.FullName);
                // Lưu token và thông tin người dùng vào Session
                HttpContext.Session.SetString("JwtToken", result.Data?.Token?? "");
                HttpContext.Session.SetString("UserName", result.Data?.User?.FullName?? "");
                HttpContext.Session.SetString("UserEmail", result.Data?.User?.Email?? "");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = result?.Message;
                return View();
            }
        }

        [HttpPost]

        public async Task<IActionResult> Register(string email, string password, string username)
        {
            var result= await _authClient.RegisterAsync(email, password, username);
            if (result.Success)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập lại";
                return RedirectToAction("Login", "Auth");

            }
            else
            {
                ViewBag.Error = result.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
