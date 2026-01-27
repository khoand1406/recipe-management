using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.Services.Bookmarks;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Application.Services.Comments;
using RecipeMgt.Application.Services.Recipes;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.RequestEntity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeServices _services;
        private readonly ICommentServices _commentServices;
        private readonly IBookmarkService _bookmarkService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<RecipeController> _logger;
        private IValidator<CreateRecipeRequest> _createRecipeValidator;
        private readonly IValidator<UpdateRecipeRequest> _updateRecipeValidator;

        public RecipeController(IRecipeServices services, ILogger<RecipeController> logger, IValidator<CreateRecipeRequest> validator, IValidator<UpdateRecipeRequest> updateValidator, ICloudinaryService cloudinaryService, ICommentServices commentServices, IBookmarkService bookmarkService)
        {
            _services = services;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _createRecipeValidator = validator;
            _updateRecipeValidator = updateValidator;
            _bookmarkService = bookmarkService;
            _commentServices = commentServices;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            try
            {
                var result = await _services.GetRecipeById(id);
                if (result == null)
                    return NotFound(new { message = $"Recipe with id {id} not found" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dish/{id}")]
        public async Task<IActionResult> getRecipes(int id)
        {
            try
            {
                var result = await _services.GetRecipesByDish(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpGet("")]

        public async Task<IActionResult> getRecipesByUser()
        {
            var userId = HttpContext.Items["UserId"] as int?;
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }
            var result = await _services.GetRecipesByUser((int)userId);

            return Ok(result);
        }

        [HttpGet("by-filter")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RecipeResponse>>>> GetRecipeByFilter()
        {
            var result = await _services.GetRecipesByFilter();

            return Ok(new ApiResponse<IEnumerable<RecipeResponse>>
            {
                Success = true,
                Data = result,
                Message = "Get recipes by filter successfully",
                RequestId = HttpContext.TraceIdentifier
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] Domain.RequestEntity.SearchRecipeRequest request)
        {
            var results = await _services.GetSearchResult(request);
            return Ok(new ApiResponse<Domain.RequestEntity.PagedResponse<Recipe>>
            {
                Success = true,
                Data = results,

            });
        }

        [Authorize]
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromServices] ICloudinaryService cloudinaryService)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded" });

            try
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(file);
                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully",
                    imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Image upload failed",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<RecipeResponse>>> CreateRecipe(CreateRecipeRequest request)
        {

            var userId = HttpContext.Items["UserId"] as int?;
            if (userId == null)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User not found",
                    RequestId = HttpContext.TraceIdentifier
                });
            }
        
            request.AuthorId = userId.Value;
            var validationResult = await _createRecipeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList(),
                    RequestId = HttpContext.TraceIdentifier
                });
            }
            var result = await _services.CreateRecipeAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = result.Error,
                    RequestId = HttpContext.TraceIdentifier
                });
            }

            return Ok(new ApiResponse<RecipeResponse>
            {
                Success = true,
                Data = result.Value,
                RequestId = HttpContext.TraceIdentifier
            });
        }


        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRecipe([FromBody] UpdateRecipeRequest request)
        {
            var userId = HttpContext.Items["UserId"] as int?;
            if (userId is null)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = AuthenticationError.AuthenError,
                    RequestId = HttpContext.TraceIdentifier
                });

            var validation = await _updateRecipeValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList(),
                    RequestId = HttpContext.TraceIdentifier
                });

            var result = await _services.UpdateRecipeAsync(
                request,
                userId.Value
            );

            if (result.IsFailure)
                return result.Error switch
                {
                    RecipeErrorMessage.NotFound =>
                        NotFound(new ApiResponse
                        {
                            Success = false,
                            Message = result.Error,
                            RequestId = HttpContext.TraceIdentifier
                        }),

                    RecipeErrorMessage.Forbidden =>
                        Forbid(),

                    _ =>
                        BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = result.Error,
                            RequestId = HttpContext.TraceIdentifier
                        })
                };

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Update recipe successfully",
                RequestId = HttpContext.TraceIdentifier
            });
        }


        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            
                var result = await _services.DeleteRecipe(id);
            if (result.IsFailure)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));
            return Ok(ApiResponseFactory.Success("Delete Successfully", HttpContext));
            
            
        }

        [Authorize]
        [HttpPost("{recipeId}/comment")]
        public async Task<IActionResult> AddComment(int recipeId, [FromBody] string content)
        {
            var userId = int.Parse(User.FindFirst("nameid")?.Value!);
            await _commentServices.AddCommentAsync(userId, recipeId, content);
            return Ok(new { message = "Comment added!" });
        }

        [Authorize]
        [HttpPost("{recipeId}/bookmark")]
        public async Task<IActionResult> AddBookmark(int recipeId)
        {
            var userId = int.Parse(User.FindFirst("nameid")?.Value!);
            var added = await _bookmarkService.AddBookmarkAsync(userId, recipeId);
            if (added.IsFailure) return BadRequest(new { message = "Already bookmarked" });
            return Ok(new { message = "Bookmarked!" });
        }

        [HttpGet("{recipeId}/comments")]
        public async Task<IActionResult> GetComments(int recipeId)
        {
            var comments = await _commentServices.GetCommentsAsync(recipeId);
            return Ok(comments);
        }
    }
}
