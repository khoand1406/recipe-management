using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Extension;
using RecipeMgt.Application.Services.Statistics.User;
using RecipeMgt.Application.Services.Users;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserStatisticService _userStatisticService;

        public UserController(IUserService userService, IUserStatisticService userStatisticService)
        {
            _userService = userService;
            _userStatisticService = userStatisticService;
        }

        [HttpGet("me/followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var userId = HttpContext.GetUserId();
            var followers = await _userService.GetFollowersAsync(userId);
            return Ok(ApiResponseFactory.Success(followers, HttpContext));
        }

        [Authorize]
        [HttpGet("me/followings")]
        public async Task<IActionResult> GetFollowing()
        {
            var userId = HttpContext.GetUserId();
            var following = await _userService.GetFollowingAsync(userId);
            return Ok(ApiResponseFactory.Success(following, HttpContext));
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetUserFollowers(int userId)
        {
            var followers = await _userService.GetFollowersAsync(userId);
            return Ok(ApiResponseFactory.Success(followers, HttpContext));
        }

        [HttpGet("{userId}/followings")]
        public async Task<IActionResult> GetUserFollowings(int userId)
        {
            var followings = await _userService.GetFollowingAsync(userId);
            return Ok(ApiResponseFactory.Success(followings, HttpContext));
        }


        [Authorize]
        [HttpPost("follow/{followingId}")]
        public async Task<IActionResult> ToggleFollow(int followingId)
        {
            var followerId = HttpContext.GetUserId();
            var isNowFollowing = await _userService.ToggleFollowAsync(followerId, followingId);

            if (isNowFollowing)
            {
                await _userStatisticService.UserFollowed(followingId);
            }
            else
            {
                await _userStatisticService.UserUnfollowed(followingId);
            }

            return Ok(ApiResponseFactory.Success(new {isFollowing= isNowFollowing }, HttpContext));
        }

        [Authorize]
        [HttpGet("me/is-following/{followingId}")]
        public async Task<IActionResult> IsFollowing( int followingId)
        {
            var followerId= HttpContext.GetUserId();
            var result = await _userService.IsFollowingAsync(followerId, followingId);
            return Ok(ApiResponseFactory.Success(new { isFollowing = result }, HttpContext));
        }
    }
}
