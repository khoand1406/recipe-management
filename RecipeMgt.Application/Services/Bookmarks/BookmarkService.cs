using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Comments;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Bookmarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Bookmarks
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _repository;

        public BookmarkService(IBookmarkRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> AddBookmarkAsync(int userId, int recipeId)
        {
            if (await _repository.IsBookmarkedAsync(userId, recipeId))
                return Result.Failure("Already bookmark");

            var bookmark = new Bookmark
            {
                UserId = userId,
                RecipeId = recipeId
            };

            await _repository.AddBookmarkAsync(bookmark);
            return Result.Success();
        }

        public async Task<Result<List<BookmarkResponseDto>>> GetBookmarksByUserAsync(int userId)
        {
            var bookmarks = await _repository.GetBookmarksByUserAsync(userId);

            var listDTO= bookmarks.Select(b => new BookmarkResponseDto
            {
                BookmarkId = b.BookmarkId,
                RecipeId = b.RecipeId,
                UserId = b.UserId,
                RecipeTitle = b.Recipe?.Title ?? "Unknown",
                RecipeThumbnail = "", 
                CreatedAt = b.CreatedAt
            }).ToList();
            return Result<List<BookmarkResponseDto>>.Success(listDTO);
        }

        public async Task<Result> RemoveBookmarkAsync(int recipeId, int userId)
        {
            var result = await _repository.RemoveAsync(userId, recipeId);
            if (!result) return Result.Failure("Remove Failed");
            return Result.Success();

        }
    }
}
