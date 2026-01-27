using RecipeMgt.Application.DTOs;
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
        Task<Result<RatingResponse>> AddOrUpdateRatingAsync(AddRatingRequest request, int userId);
        Task<Result<double>> GetAverageRatingAsync(int recipeId);
        Task<Result<UserRatingResponse?>> GetUserRatingAsync(int userId, int recipeId);
    }
}
