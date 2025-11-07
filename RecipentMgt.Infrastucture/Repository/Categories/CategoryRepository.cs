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
            return await _context.Categories.ToListAsync();
        }


    }
}
