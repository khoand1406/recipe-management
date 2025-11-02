using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Bookmarks
{
    public interface IBookmarkRepository
    {
        Task AddBookmarkAsync(Bookmark bookmark);
        Task<bool> IsBookmarkedAsync(int userId, int recipeId);

        Task<List<Bookmark>> GetBookmarksByUserAsync(int userId);
    }
}
