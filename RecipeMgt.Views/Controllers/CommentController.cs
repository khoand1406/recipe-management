using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Models.RequestModel;

namespace RecipeMgt.Views.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly CommentClient _commentClient;
        private readonly IConfiguration _configuration;

        public CommentController(ILogger<CommentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            _commentClient = new CommentClient(apiBaseUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            var comments = await _commentClient.GetCommentsByRecipeIdAsync(recipeId);
            return Json(comments);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int recipeId, string content)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _commentClient.SetBearerToken(token);
            
            var result = await _commentClient.AddCommentAsync(recipeId, content);
            return Json(result);
        }
    }
}
