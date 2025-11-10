using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(result);
        }
    }
}
