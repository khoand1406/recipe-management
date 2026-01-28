using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Statistics.User
{
    public interface IUserStatisticService
    {
        Task UserCreatedRecipe(int userId);

        Task UserFollowed(int userId);

        Task UserRated(int userId);
    }
}
