using RecipeMgt.Application.DTOs.Request.Rating;
using RecipeMgt.Application.DTOs.Response.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Ratings
{
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using RecipeMgt.Application.DTOs.Request.Rating;
    using RecipeMgt.Application.DTOs.Response.Rating;
    using RecipeMgt.Domain.Entities;
    using RecipentMgt.Infrastucture.Repository.Ratings;
    using System;
    using System.Threading.Tasks;

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RatingService> _logger;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper, ILogger<RatingService> logger)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RatingResponse> AddOrUpdateRatingAsync(AddRatingRequest request, int userId)
        {
            try
            {
                var rating = new Rating
                {
                    RecipeId = request.RecipeId,
                    UserId = userId,
                    Score = request.Score,
                    Comment = request.Comment,
                    
                };

                await _ratingRepository.AddOrUpdateRatingAsync(rating);

                double avg = await _ratingRepository.GetAverageRatingAsync(request.RecipeId) ?? 0;

                return new RatingResponse
                {
                    Success = true,
                    Message = "Rating saved successfully",
                    AverageRating = avg
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving rating");
                return new RatingResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<double> GetAverageRatingAsync(int recipeId)
        {
            return await _ratingRepository.GetAverageRatingAsync(recipeId)?? 0;
        }

        public async Task<UserRatingResponse?> GetUserRatingAsync(int userId, int recipeId)
        {
            var rating = await _ratingRepository.GetUserRatingAsync(userId, recipeId);
            if (rating == null) return null;

            return new UserRatingResponse
            {
                RecipeId = recipeId,
                UserId = userId,
                Score = rating.Score?? 0,
                Comment = rating.Comment
            };
        }
    }

}
