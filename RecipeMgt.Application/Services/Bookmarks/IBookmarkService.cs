using RecipeMgt.Application.DTOs.Response.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Bookmarks
{
    public interface IBookmarkService
    {
        Task<bool> AddBookmarkAsync(int userId, int recipeId);

        Task<List<BookmarkResponseDto>> GetBookmarksByUserAsync(int userId);
    }
}
