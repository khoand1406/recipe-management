using RecipeMgt.Application.DTOs.Response.Comments;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Comments
{
    public class CommentServices : ICommentServices
    {
        private readonly ICommentRepository _repository;

        public CommentServices(ICommentRepository repository)
        {
            _repository = repository;
        }

        public async Task AddCommentAsync(int userId, int recipeId, string content)
        {
            var comment = new Comment
            {
                UserId = userId,
                RecipeId = recipeId,
                Content = content
            };
            await _repository.AddCommentAsync(comment);
        }

        public async Task<List<CommentResposneDTO>> GetCommentsAsync(int recipeId)
        {
            var comments = await _repository.GetCommentsByRecipeIdAsync(recipeId);

            return comments.Select(c => new CommentResposneDTO
            {
                CommentId = c.CommentId,
                RecipeId = c.RecipeId,
                UserId = c.UserId,
                UserName = c.User?.FullName ?? "Unknown",
               
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public Task<bool> RemoveAsync(int userId, int recipeId)
        {
            throw new NotImplementedException();
        }
    }
}
