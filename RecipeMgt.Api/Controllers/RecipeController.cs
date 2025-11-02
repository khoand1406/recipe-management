using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            _commentServices= commentServices;
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
        [Consumes("multipart/form-data")]
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<CreateRecipeResponse>>> CreateRecipe([FromForm] CreateRecipeRequest request)
        {
            try
            {
                // ✅ Lấy UserId từ middleware
                var userId = HttpContext.Items["UserId"] as int?;
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse<CreateRecipeResponse>
                    {
                        Success = false,
                        Message = "User not found",
                        RequestId = HttpContext.TraceIdentifier
                    });
                }

                // ✅ Parse Ingredients & Steps từ JSON text sang List<>
                request.Ingredients = JsonSerializer.Deserialize<List<IngredientDto>>(request.IngredientsJson ?? "[]") ?? new();
                request.Steps = JsonSerializer.Deserialize<List<StepDto>>(request.StepsJson ?? "[]") ?? new();

                // ✅ Gán AuthorId
                request.AuthorId = userId.Value;

                // ✅ Validate dữ liệu
                var validationResult = await _createRecipeValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new ApiResponse<CreateRecipeResponse>
                    {
                        Success = false,
                        Message = "Validation Failed",
                        Errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }

                // ✅ Upload ảnh lên Cloudinary (nếu có)
                if (request.Images != null && request.Images.Any())
                {
                    request.ImageUrls = new List<string>();
                    foreach (var file in request.Images)
                    {
                        var uploadedUrl = await _cloudinaryService.UploadImageAsync(file);
                        request.ImageUrls.Add(uploadedUrl);
                    }
                }

                // ✅ Gọi service tạo Recipe
                var result = await _services.CreateRecipe(request);
                if (result.Success)
                {
                    return Ok(new ApiResponse<CreateRecipeResponse>
                    {
                        Success= true,
                        Data= result,
                    });
                }

                // Nếu service báo lỗi
                return BadRequest(new ApiResponse<CreateRecipeResponse>
                {
                    Success = false,
                    Message = result.Message ?? "Failed to create recipe",
                    RequestId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                // ✅ Exception
                return StatusCode(500, new ApiResponse<CreateRecipeResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                });
            }
        }
        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<UpdateRecipeResponse>>> updateRecipe([FromForm] UpdateRecipeRequest request)
        {
            try
            {
                var userId = HttpContext.Items["UserId"] as int?;
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }
                request.Ingredients = JsonSerializer.Deserialize<List<IngredientDto>>(request.IngredientsJson ?? "[]") ?? new();
                request.Steps = JsonSerializer.Deserialize<List<StepDto>>(request.StepsJson ?? "[]") ?? new();

                var validation = await _updateRecipeValidator.ValidateAsync(request);
                if (!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<UpdateRecipeResponse>
                    {
                        Success = false,

                        Message = "Validaton Failed",
                        Errors = validation.Errors
                                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                            .ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                var userId_parsed = userId.Value;
                request.AuthorId = userId_parsed;

                if (userId_parsed != request.AuthorId)
                {
                    return Forbid("Can't update other user's recipes");
                }

                if (request.Images != null && request.Images.Any())
                {
                    request.ImageUrls = new List<string>();
                    foreach (var file in request.Images)
                    {
                        var uploadedUrl = await _cloudinaryService.UploadImageAsync(file);
                        request.ImageUrls.Add(uploadedUrl);
                    }
                }
                var result = await _services.UpdateRecipe(request);
                if (result.Success)
                {
                    return new ApiResponse<UpdateRecipeResponse>
                    {
                        Success = true,

                        RequestId = HttpContext.TraceIdentifier

                    };
                }
                return new ApiResponse<UpdateRecipeResponse>
                {
                    Success = false,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating recipe: " + ex.Message);
                return BadRequest(new ApiResponse<UpdateRecipeResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                });
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ApiResponse<DeleteRecipeResponse>>> DeleteRecipe(int id)
        {
            try
            {
                var result = await _services.DeleteRecipe(id);
                if (result.Success)
                {
                    return new ApiResponse<DeleteRecipeResponse>
                    {
                        Success = true,
                        Message = result.Message,
                    };

                }
                return new ApiResponse<DeleteRecipeResponse>
                {
                    Success = false,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during delete recipe: " + ex.Message);
                return BadRequest(new ApiResponse<DeleteRecipeResponse>()
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
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
            if (!added) return BadRequest(new { message = "Already bookmarked" });
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
