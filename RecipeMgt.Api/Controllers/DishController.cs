using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Extension;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.Services.Dishes;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        // GET: api/dish?page=1&pageSize=10&searchQuery=&categoryId=
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] int? categoryId = null)
        {
            var result = await _dishService.getDishes(
                page, pageSize, searchQuery, categoryId);

            if (result.IsFailure)
                return BadRequest(
                    ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // GET: api/dish/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            
            var userId = HttpContext.GetOptionalUserId();
            string? sessionId = null;
            if (userId == null)
            {
                sessionId = GetOrCreateSessionId();
            }

            var result = await _dishService.GetDishDetail(id, userId, sessionId);

            if (result.IsFailure)
                return NotFound(
                    ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpGet("top-view")]
        public async Task<IActionResult> GetTopViewDishes()
        {
            var result = await _dishService.GetTopViewCount();
            if (result.IsFailure)
                return BadRequest(
                    ApiResponseFactory.Fail(result.Error, HttpContext));
            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext));
        }



        private string GetOrCreateSessionId()
        {
            const string cookieKey= "guest_session_id";
            if(Request.Cookies.TryGetValue(cookieKey, out var sessionId))
            {
                return sessionId!;
            }
            else
            {
                var newSessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append(cookieKey, newSessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
                return newSessionId;
            }
        }

        // POST: api/dish/create
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDish(
            [FromForm] CreateDishRequest request)
        {
            var userId= HttpContext.GetOptionalUserId();
            request.AuthorId = userId;
            var result = await _dishService.CreateDish(request);

            if (result.IsFailure)
                return BadRequest(
                    ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(
                ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // PATCH: api/dish/update
        [HttpPatch("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDish(
            [FromForm] UpdateDishRequest request)
        {
            var result = await _dishService.UpdateDish(request);

            if (result.IsFailure)
                return BadRequest(
                    ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(
                ApiResponseFactory.Success("Update dish successfully", HttpContext));
        }

        // DELETE: api/dish/delete/5
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var result = await _dishService.DeleteDish(id);

            if (result.IsFailure)
                return BadRequest(
                    ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(
                ApiResponseFactory.Success("Delete dish successfully", HttpContext));
        }
    }
}
