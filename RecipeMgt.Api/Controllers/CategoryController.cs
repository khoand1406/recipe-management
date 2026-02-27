using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Application.Services.Dishes;
using RecipentMgt.Infrastucture.Repository.Categories;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IDishService _service;
        public CategoryController(IDishService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetCategoriesWithDishes();
            return result.IsSuccess ? Ok(ApiResponseFactory.Success(result.Value, HttpContext)) : BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));
        }
    }
}
