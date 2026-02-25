using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;

namespace RecipeMgt.Views.Controllers
{
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentClient _commentClient;
        

        public CommentController(ILogger<CommentController> logger, ICommentClient commentClient)
        {
            _commentClient = commentClient;
            _logger = logger;
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
