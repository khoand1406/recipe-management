using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipentMgt.Infrastucture.Repository.Categories;

namespace RecipeMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository repository;

        public CategoryController(ICategoryRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var result= await repository.GetAll();
            var mappedResult = result.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Dishes = c.Dishes.Select(d => new DishBasicResponse
                {
                    DishId = d.DishId,
                    DishName = d.DishName,
                    Images = d.Images?.Select(i => i.ImageUrl).ToArray(),
                    CategoryId = d.CategoryId,
                }).ToList()
            });
            return Ok(result);
        }
    }
}
