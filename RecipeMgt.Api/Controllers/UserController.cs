using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.Services.Users;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(int userId)
        {
            var followers = await _userService.GetFollowersAsync(userId);
            return Ok(followers);
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            var following = await _userService.GetFollowingAsync(userId);
            return Ok(following);
        }

        [HttpPost("{followerId}/follow/{followingId}")]
        public async Task<IActionResult> ToggleFollow(int followerId, int followingId)
        {
            var isNowFollowing = await _userService.ToggleFollowAsync(followerId, followingId);
            return Ok(new { success = true, following = isNowFollowing });
        }

        [HttpGet("{followerId}/is-following/{followingId}")]
        public async Task<IActionResult> IsFollowing(int followerId, int followingId)
        {
            var result = await _userService.IsFollowingAsync(followerId, followingId);
            return Ok(new { isFollowing = result });
        }
    }
}
