using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.Services.Recipes;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeServices _services;
        private readonly ILogger<RecipeController> _logger;
        private IValidator<CreateRecipeRequest> _createRecipeValidator;
        private readonly IValidator<UpdateRecipeRequest> _updateRecipeValidator;

        public RecipeController(IRecipeServices services, ILogger<RecipeController> logger, IValidator<CreateRecipeRequest> validator, IValidator<UpdateRecipeRequest> updateValidator)
        {
            _services = services;
            _logger = logger;
            _createRecipeValidator = validator;
            _updateRecipeValidator = updateValidator;
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

        [Authorize]
        [HttpPost("create")]

        public async Task<ActionResult<ApiResponse<CreateRecipeResponse>>> CreateRecipe([FromBody] CreateRecipeRequest request)
        {
            try
            {
                var userId = HttpContext.Items["UserId"] as int?;
                Console.WriteLine(userId);
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var validator = await _createRecipeValidator.ValidateAsync(request);
                if (!validator.IsValid)
                {
                    return BadRequest(new ApiResponse<CreateRecipeResponse>
                    {
                        Success = false,
                        Message = "Validaton Failed",
                        Errors = validator.Errors
                                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                            .ToList(),
                        RequestId = HttpContext.TraceIdentifier
                    });

                }

                request.AuthorId = userId ?? 0;

                var result = await _services.CreateRecipe(request);
                if (result.Success)
                {
                    return new ApiResponse<CreateRecipeResponse>
                    {
                        Success = true,

                        RequestId = HttpContext.TraceIdentifier
                    };
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return new ApiResponse<CreateRecipeResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<UpdateRecipeResponse>>> updateRecipe([FromBody] UpdateRecipeRequest request)
        {
            try
            {
                var userId = HttpContext.Items["UserId"] as int?;
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

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
    }
}
