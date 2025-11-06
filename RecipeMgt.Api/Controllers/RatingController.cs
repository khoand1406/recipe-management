using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Request.Rating;
using RecipeMgt.Application.Services.Ratings;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private IValidator<AddRatingRequest> _ratingValidator;

        public RatingController(IRatingService ratingService, IValidator<AddRatingRequest> validator)
        {
            _ratingService = ratingService;
            _ratingValidator = validator;
        }


        [Authorize]
        [HttpPost("rate")]
        public async Task<IActionResult> RateRecipe([FromBody] AddRatingRequest request)
        {
            var validation = await _ratingValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation Failed",
                    errors = validation.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                });
            }

            var userId = HttpContext.Items["UserId"] as int?;
            if (userId == null)
                return Unauthorized("User not found!");

            var result = await _ratingService.AddOrUpdateRatingAsync(request, userId.Value);
            return Ok(result);
        }


        [HttpGet("average/{recipeId}")]
        public async Task<IActionResult> GetAverageRating(int recipeId)
        {
            double avg = await _ratingService.GetAverageRatingAsync(recipeId);
            return Ok(new { recipeId, averageRating = avg });
        }

        [Authorize]
        [HttpGet("user/{recipeId}")]
        public async Task<IActionResult> GetUserRating(int recipeId)
        {
            var userId = HttpContext.Items["UserId"] as int?;
            if (userId == null)
                return Unauthorized(new { message = "User not found!" });

            var result = await _ratingService.GetUserRatingAsync(userId.Value, recipeId);

            if (result == null)
                return Ok(new { message = "User has not rated this recipe yet" });

            return Ok(result);
        }
    }
}
