using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Following
{
    public interface IFollowingRepository
    {
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task FollowAsync(RecipeMgt.Domain.Entities.Following follow);
        Task UnfollowAsync(int followerId, int followingId);

        Task<List<User>> GetFollowers(int userId);

        Task<List<User>> GetFollowingUsers(int userId);
    }
}
