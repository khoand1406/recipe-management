using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.Services.Bookmarks;
using RecipentMgt.Infrastucture.Repository.Users;
using System.Security.Claims;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BookmarkController> _logger;

        public BookmarkController(IBookmarkService bookmarkService, IUserRepository userRepository, ILogger<BookmarkController> logger)
        {
            _bookmarkService = bookmarkService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("my-bookmarks")]
        public async Task<IActionResult> GetMyBookmarks()
        {
            try
            {
                // Get email from JWT claims
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { message = "User email not found in token" });
                }

                // Query database to get actual UserId
                var user = await _userRepository.getUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found in database" });
                }

                var bookmarks = await _bookmarkService.GetBookmarksByUserAsync(user.UserId);
                return Ok(bookmarks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user bookmarks");
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("{recipeId}")]
        public async Task<IActionResult> AddBookmark(int recipeId)
        {
            try
            {
                // Get email from JWT claims
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { message = "User email not found in token" });
                }

                // Query database to get actual UserId
                var user = await _userRepository.getUserByEmail(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found in database" });
                }

                // Add bookmark using database UserId
                var added = await _bookmarkService.AddBookmarkAsync(user.UserId, recipeId);
                if (!added)
                {
                    return BadRequest(new { message = "Already bookmarked or recipe not found" });
                }

                return Ok(new { message = "Recipe bookmarked successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bookmark");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
