using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Ratings
{
    public class RatingRepository : IRatingRepository
    {
        private readonly RecipeManagementContext _context;
        private ILogger<RatingRepository> _logger;
        public RatingRepository(RecipeManagementContext context, ILogger<RatingRepository> logger) {
            _context = context;
            _logger = logger;
        }
        public async Task AddOrUpdateRatingAsync(Rating rating)
        {
            try
            {
                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.RecipeId == rating.RecipeId);

                if (existingRating != null)
                {
                    // Update
                    existingRating.Score = rating.Score;
                    existingRating.Comment = rating.Comment;
                    
                }
                else
                {
                    // Insert
                    rating.CreatedAt = DateTime.UtcNow;
                    await _context.Ratings.AddAsync(rating);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding/updating rating for RecipeId = {rating.RecipeId}");
                throw;
            }
        }

        public async Task<Rating?> GetUserRatingAsync(int userId, int recipeId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);
        }

        public async Task<double?> GetAverageRatingAsync(int recipeId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();

            if (ratings.Count == 0) return 0;
            return ratings.Average(r => r.Score);
        }
    }
}
