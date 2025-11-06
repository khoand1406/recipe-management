using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Comments
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RecipeManagementContext _recipeManagementContext;

        public CommentRepository(RecipeManagementContext recipeManagementContext)
        {
            _recipeManagementContext = recipeManagementContext;
        }
        public async Task AddCommentAsync(Comment comment)
        {
            _recipeManagementContext.Comment.Add(comment);
            await _recipeManagementContext.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId)
        {
            return await _recipeManagementContext.Comment
            .Where(c => c.RecipeId == recipeId)
            .Include(c => c.User)
            .ToListAsync();
        }
    }
}
