using Microsoft.Extensions.Logging;
using RecipentMgt.Infrastucture.Repository.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Statistics.User
{
    public class UserStatisticService : IUserStatisticService
    {
        private readonly IStatisticRepository _statisticRepository;
        private ILogger<UserStatisticService> _logger;

        public UserStatisticService(IStatisticRepository statisticRepository, ILogger<UserStatisticService> logger )
        {
            _statisticRepository = statisticRepository;
            _logger = logger;
        }

        public async Task UserCreatedRecipe(int userId)
        {
            try
            {
                await _statisticRepository.IncreaseUserRecipeAsync(userId);
            }catch(Exception ex)
            {
                _logger.LogError(
                    ex,
                    "UserStatistic UserCreatedRecipe failed. UserId={UserId}",
                    userId
                );
            }
        }

        public async Task UserFollowed(int userId)
        {
            try
            {
                await _statisticRepository.IncreaseUserFollowerAsync(userId);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "UserFollew Failed. UserId= {UserId}", userId);
            }
        }

        public async Task UserRated(int userId)
        {
            try
            {
                await _statisticRepository.IncreaseUserRatingAsync(userId);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "User Rating Failed. UserId= {UserId}", userId);
            }
        }

        public async Task UserUnfollowed(int userId)
        {
            try
            {
                await _statisticRepository.DecreaseUserFollowerAsync(userId);
            }catch(Exception ex )
            {
                _logger.LogError(ex, "User UnFollew Failed. UserId= {UserId}", userId);
            }
        }
    }
}
