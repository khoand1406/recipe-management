using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Extension;
using RecipeMgt.Application.DTOs.Request.Rating;
using RecipeMgt.Application.Services.Ratings;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IValidator<AddRatingRequest> _ratingValidator;

        public RatingController(
            IRatingService ratingService,
            IValidator<AddRatingRequest> validator)
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
                return BadRequest(
                    ApiResponseFactory.Fail(validation, HttpContext)
                );
            }

            var userId = HttpContext.GetUserId();
            var result = await _ratingService.AddOrUpdateRatingAsync(
                request, userId
            );

            if (!result.IsSuccess)
            {
                return BadRequest(
                    ApiResponseFactory.Fail(
                        result.Error,
                        HttpContext
                        )
                );
            }

            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext)
            );
        }



        [HttpGet("average/{recipeId}")]
        public async Task<IActionResult> GetAverageRating(int recipeId)
        {
            var avg = await _ratingService.GetAverageRatingAsync(recipeId);

            return Ok(
                ApiResponseFactory.Success(
                    new
                    {
                        RecipeId = recipeId,
                        AverageRating = avg
                    },
                    HttpContext
                )
            );
        }



        [Authorize]
        [HttpGet("user/{recipeId}")]
        public async Task<IActionResult> GetUserRating(int recipeId)
        {
            var userId = HttpContext.GetUserId();
            

            var result = await _ratingService.GetUserRatingAsync(
                userId, recipeId
            );

            if (!result.IsSuccess)
            {
                return NotFound(
                    ApiResponseFactory.Fail(
                        result.Error,
                        HttpContext
                    )
                );
            }

            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext)
            );
        }
    }
}
