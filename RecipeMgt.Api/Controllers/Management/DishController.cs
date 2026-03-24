using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Extension;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.Services.Dishes;

namespace RecipeMgt.Api.Controllers.Management
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IMapper _mapper;

        public DishController(IDishService service, IMapper mapper)
        {
            _dishService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
        string? searchQuery,
        int? categoryId,
        int page = 1,
        int pageSize = 10)
        {
            var result = await _dishService.GetDishesWithStat(page, pageSize, searchQuery, categoryId);

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // GET: api/admin/dishes/{id}
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

            if (!result.IsSuccess)
                return NotFound(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // POST: api/admin/dishes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDishRequest request)
        {
            var result = await _dishService.CreateDish(request);

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // PUT: api/admin/dishes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDishRequest request)
        {
            var result = await _dishService.UpdateDish(request);

            if (!result.IsSuccess)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success(result.IsSuccess, HttpContext));
        }

        // DELETE: api/admin/dishes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dishService.DeleteDish(id);

            if (!result.IsSuccess)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success("DELETE_SUCCESS", HttpContext));
        }

        [HttpPost("{id:int}/approved")]
        public async Task<IActionResult> ApproveDish(int id)
        {
            var result = await _dishService.ApproveDish(id);
            return Ok(ApiResponseFactory.Success("APPROVE_SUCCESS", HttpContext));
        }

        [HttpPost("{id:int}/declined")]
        public async Task<IActionResult> Declined(int id)
        {
            var result= await _dishService.RejectDish(id);
            return Ok(ApiResponseFactory.Success("DECLINED_SUCCESS", HttpContext));
        }

        private string GetOrCreateSessionId()
        {
            const string cookieKey = "guest_session_id";
            if (Request.Cookies.TryGetValue(cookieKey, out var sessionId))
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
    }
}
