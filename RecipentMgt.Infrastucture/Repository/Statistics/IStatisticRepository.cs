using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Statistics
{
    public interface IStatisticRepository
    {
        Task IncreaseRecipeViewAsync(int recipeId);
        Task IncreaseRecipeBookmarkAsync(int recipeId);
        Task IncreaseRecipeCommentAsync(int recipeId);
        Task UpdateRecipeRatingAsync(int recipeId, int ratingValue);

        Task IncreaseUserRecipeAsync(int userId);
        Task IncreaseUserCommentAsync(int userId);
        Task IncreaseUserRatingAsync(int userId);
        Task IncreaseUserFollowerAsync(int userId);

        Task DecreasRecipeBookmarkAsync(int recipeId);

        Task DecreaseUserFollowerAsync(int userId);

        Task IncreaseDishBookmarkCount(int dishId);

        Task IncreaseDishViewCount(int dishId);

        Task DecreaseDishBookmarkCount(int dishId);
    }
}
