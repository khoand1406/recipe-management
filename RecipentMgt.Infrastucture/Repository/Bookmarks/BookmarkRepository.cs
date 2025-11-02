using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Bookmarks
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly RecipeManagementContext _context;
        public BookmarkRepository(RecipeManagementContext context)
        {
            _context = context;
        }
        public async Task AddBookmarkAsync(Bookmark bookmark)
        {
            await _context.AddAsync(bookmark);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> IsBookmarkedAsync(int userId, int recipeId)
        {
            return await _context.Bookmarks.AnyAsync(b => b.UserId == userId && b.RecipeId == recipeId);
        }
    }
}
