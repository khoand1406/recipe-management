using RecipeMgt.Application.DTOs.Request.Rating;
using RecipeMgt.Application.DTOs.Response.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Ratings
{
    public interface IRatingService
    {
        Task<RatingResponse> AddOrUpdateRatingAsync(AddRatingRequest request, int userId);
        Task<double> GetAverageRatingAsync(int recipeId);
        Task<UserRatingResponse?> GetUserRatingAsync(int userId, int recipeId);
    }
}
