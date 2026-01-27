using AutoMapper;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Comments;
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
        private readonly IMapper _mapper;

        public CommentServices(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result> AddCommentAsync(int userId, int recipeId, string content)
        {
            var comment = new CreateCommentDTO
            {
                UserId = userId,
                RecipeId = recipeId,
                Content = content, 
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };
            var mapped = _mapper.Map<Comment>(comment);
            await _repository.AddCommentAsync(mapped);
            return Result.Success();

        }

        public async Task<Result<List<CommentResposneDTO>>> GetCommentsAsync(int recipeId)
        {
            var comments = await _repository.GetCommentsByRecipeIdAsync(recipeId);

            var listDTO=  comments.Select(c => new CommentResposneDTO
            {
                CommentId = c.CommentId,
                RecipeId = c.RecipeId,
                UserId = c.UserId,
                UserName = c.User?.FullName ?? "Unknown",
               
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();
            return Result<List<CommentResposneDTO>>.Success(listDTO);
        }

        public Task<Result> RemoveAsync(int userId, int recipeId)
        {
            throw new NotImplementedException();
        }
    }
}
