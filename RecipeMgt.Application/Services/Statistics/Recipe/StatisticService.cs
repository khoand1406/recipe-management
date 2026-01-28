using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs;
using RecipentMgt.Infrastucture.Repository.Statistics;

namespace RecipeMgt.Application.Services.Statistics.Recipe
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _statisticRepository;
        private readonly ILogger<StatisticService> _logger;

        public StatisticService(
            IStatisticRepository statisticRepository,
            ILogger<StatisticService> logger)
        {
            _statisticRepository = statisticRepository;
            _logger = logger;
        }

        public async Task RecipeComment(int recipeId, int userId)
        {
            try
            {
                await Task.WhenAll(
                    _statisticRepository.IncreaseRecipeCommentAsync(recipeId),
                    _statisticRepository.IncreaseUserCommentAsync(userId)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Statistic RecipeComment failed. RecipeId={RecipeId}, UserId={UserId}",
                    recipeId,
                    userId
                );
            }
        }

        public async Task RecipeRated(int recipeId, int userId, int recipeRating)
        {
            try
            {
                await Task.WhenAll(
                    _statisticRepository.UpdateRecipeRatingAsync(recipeId, recipeRating),
                    _statisticRepository.IncreaseUserRatingAsync(userId)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Statistic RecipeRated failed. RecipeId={RecipeId}, UserId={UserId}, Rating={Rating}",
                    recipeId,
                    userId,
                    recipeRating
                );
            }
        }

        public async Task RecipeViewd(int recipeId)
        {
            try
            {
                await _statisticRepository.IncreaseRecipeViewAsync(recipeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Statistic RecipeViewed failed. RecipeId={RecipeId}",
                    recipeId
                );
            }
        }
    }
}
