using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
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
            var result = await _dishService.getDishes(page, pageSize, searchQuery, categoryId);

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // GET: api/admin/dishes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _dishService.GetDishDetail(id);

            if (!result.Success)
                return NotFound(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // POST: api/admin/dishes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDishRequest request)
        {
            var result = await _dishService.CreateDish(request);

            if (!result.Success)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // PUT: api/admin/dishes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDishRequest request)
        {
            var result = await _dishService.UpdateDish(id, request);

            if (!result.Success)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        // DELETE: api/admin/dishes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dishService.DeleteDish(id);

            if (!result.Success)
                return BadRequest(ApiResponseFactory.Fail(result.Error, HttpContext));

            return Ok(ApiResponseFactory.Success("DELETE_SUCCESS", HttpContext));
        }
    }
}
