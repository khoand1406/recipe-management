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

        public async Task<List<Bookmark>> GetBookmarksByUserAsync(int userId)
        {
            {
                return await _context.Bookmark
                    .Include(b => b.Recipe)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
        }

        public async Task<bool> IsBookmarkedAsync(int userId, int recipeId)
        {
            return await _context.Bookmark.AnyAsync(b => b.UserId == userId && b.RecipeId == recipeId);
        }

        public async Task<bool> RemoveAsync(int userId, int recipeId)
        {
            var bookmark = await _context.Bookmark
            .FirstOrDefaultAsync(b => b.UserId == userId && b.RecipeId == recipeId);

            if (bookmark == null) return false;

            _context.Bookmark.Remove(bookmark);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
