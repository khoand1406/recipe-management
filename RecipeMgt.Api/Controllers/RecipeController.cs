using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.Services.Recipes;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("")]

        public async Task<IActionResult> getRecipesByUser()
        {
            var result = await _services.GetRecipesByUser(0);
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

        [HttpPost("create")]

        public async Task<ActionResult<ApiResponse<CreateRecipeResponse>>> CreateRecipe([FromBody] CreateRecipeRequest request)
        {
            try
            {
                var validator= await _createRecipeValidator.ValidateAsync(request);
                if (!validator.IsValid)
                {
                    return BadRequest(new ApiResponse<CreateRecipeResponse>
                    {
                        Success = false,
                        Message = "Validaton Failed",
                        Errors = (List<string>)validator.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                        RequestId = HttpContext.TraceIdentifier
                    });
                    
                }
                var result= await _services.CreateRecipe(request);
                if(result.Success)
                {
                    return new ApiResponse<CreateRecipeResponse>
                    {
                        Success = true,
                        
                        RequestId = HttpContext.TraceIdentifier
                    };
                }
                return Ok();
            }catch (Exception ex)
            {
                return new ApiResponse<CreateRecipeResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<UpdateRecipeResponse>>> updateRecipe([FromBody] UpdateRecipeRequest request)
        {
            try
            {
                var validation= await _updateRecipeValidator.ValidateAsync(request);
                if (!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<UpdateRecipeResponse>
                    {
                        Success = false,
                        
                        Message = "Validaton Failed",
                        Errors = (List<string>)validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                        RequestId = HttpContext.TraceIdentifier
                    });
                }
                var result= await _services.UpdateRecipe(request);
                if (result.Success)
                {
                    return new ApiResponse<UpdateRecipeResponse>
                    {
                        Success = true,
                        
                        RequestId = HttpContext.TraceIdentifier

                    };
                }
                return new ApiResponse<UpdateRecipeResponse> {
                    Success = false,
                };
            }catch (Exception ex)
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

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ApiResponse<DeleteRecipeResponse>>> DeleteRecipe(int id)
        {
            try
            {
                var result= await _services.DeleteRecipe(id);
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
