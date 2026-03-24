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

        public async Task<int> CountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<ICollection<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetAuthorCount()
        {
            var result = await _context.Dishes
        .GroupBy(d => d.CategoryId)
        .Select(g => new
        {
            CategoryId = g.Key,
            AuthorCount = g.Select(x => x.AuthorId).Distinct().Count()
        })
        .ToDictionaryAsync(x => x.CategoryId, x => x.AuthorCount);
            return result;

        }

        public async Task<Dictionary<int, int>> GetDishCount()
        {
            var result = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    Count = c.Dishes.Count(d => d.IsConfirm)
                })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

            return result;
        }
    }
}
