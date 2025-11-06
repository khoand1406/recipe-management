using RecipeMgt.Application.DTOs.Response.Comments;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Comments
{
    public interface ICommentServices
    {
        Task AddCommentAsync(int userId, int recipeId, string content);
        Task<List<CommentResposneDTO>> GetCommentsAsync(int recipeId);

        Task<bool> RemoveAsync(int userId, int recipeId);
    }
}
