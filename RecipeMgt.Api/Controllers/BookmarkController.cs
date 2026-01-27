using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.Constant;
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
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ApiResponseFactory.Fail(AuthenticationError.AuthenError, HttpContext));
            }

            var user = await _userRepository.getUserByEmail(email);
            if (user == null)
            {
                return NotFound(ApiResponseFactory.Fail("", HttpContext));
            }

            var bookmarks = await _bookmarkService.GetBookmarksByUserAsync(user.UserId);
            return Ok(bookmarks.Value);
        }

        [Authorize]
        [HttpPost("{recipeId}")]
        public async Task<IActionResult> AddBookmark(int recipeId)
        {
            
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                return Unauthorized(ApiResponseFactory.Fail(AuthenticationError.AuthenError, HttpContext));
                }
                var user = await _userRepository.getUserByEmail(email);
                if (user == null)
                {
                return NotFound(ApiResponseFactory.Fail("", HttpContext));
                }
                var added = await _bookmarkService.AddBookmarkAsync(user.UserId, recipeId);
                if (added.IsFailure)
                {
                    return BadRequest(ApiResponseFactory.Fail(added.Error, HttpContext));
                }

            return Ok(ApiResponseFactory.Success("Create successfully", HttpContext));
            }
            
        }
    }

