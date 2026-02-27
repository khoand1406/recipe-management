using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class AuthController : Controller
    {

        private readonly IAuthClient _authClient;
        
        public AuthController(IAuthClient authClient)
        {
            _authClient = authClient;
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

        [HttpGet]
        public IActionResult ChangePassword()
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

        [HttpPatch]

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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
