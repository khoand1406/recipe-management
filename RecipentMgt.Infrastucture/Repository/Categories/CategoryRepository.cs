using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RecipeManagementContext _context;

        public CategoryRepository(RecipeManagementContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetAll()
        {
            return await _context.Categories.Include(c=> c.Dishes).Select(c => new Category
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Dishes = c.Dishes.Select(d => new Dish
                {
                    DishId = d.DishId,
                    DishName = d.DishName,
                    Images = d.Images,
                    CategoryId = d.CategoryId
                }).OrderBy(x=> x.DishId).Take(10).ToList()
            }).ToListAsync();
        }
    }
}
