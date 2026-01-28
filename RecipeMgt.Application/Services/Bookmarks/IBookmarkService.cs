using RecipeMgt.Application.DTOs;
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
        Task<Result> AddBookmarkAsync(int userId, int recipeId);

        Task<Result<List<BookmarkResponseDto>>> GetBookmarksByUserAsync(int userId);

        Task<Result> RemoveBookmarkAsync(int bookmarkId, int userId);
    }
}
