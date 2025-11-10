using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Models.RequestModel;

namespace RecipeMgt.Views.Controllers
{
    public class BookmarkController : Controller
    {
        private readonly ILogger<BookmarkController> _logger;
        private readonly BookmarkClient _bookmarkClient;
        private readonly IConfiguration _configuration;

        public BookmarkController(ILogger<BookmarkController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            _bookmarkClient = new BookmarkClient(apiBaseUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            _bookmarkClient.SetBearerToken(token);
            var bookmarks = await _bookmarkClient.GetMyBookmarksAsync();
            return View(bookmarks);
        }

        [HttpPost]
        public async Task<IActionResult> AddBookmark(int recipeId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Please login first" });
            }

            _bookmarkClient.SetBearerToken(token);
            var result = await _bookmarkClient.AddBookmarkAsync(recipeId);
            
            // Return consistent lowercase JSON for JavaScript
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
