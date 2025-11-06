using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Ratings
{
    public interface IRatingRepository
    {
        Task<Rating?> GetUserRatingAsync(int userId, int recipeId);
        Task AddOrUpdateRatingAsync(Rating rating);
        Task<double?> GetAverageRatingAsync(int recipeId);
    }
}
