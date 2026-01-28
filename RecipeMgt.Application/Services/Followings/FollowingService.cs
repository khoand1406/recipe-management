using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Follows;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Following;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Followings
{
    public class FollowingService : IFollowingService
    {
        private readonly IFollowingRepository _repo;
        public FollowingService(IFollowingRepository followingRepository) {
            _repo = followingRepository;
        }

        public Task<Result> FollowAsync(CreateFollowDTO follow)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<UserResponseDTO>>> GetFollowers(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<UserResponseDTO>>> GetFollowingUsers(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ToggleFollowAsync(int followerId, int followingId)
        {
            if (await _repo.IsFollowingAsync(followerId, followingId))
            {
                await _repo.UnfollowAsync(followerId, followingId);
                return false;
            }
            else
            {
                await _repo.FollowAsync(new Following
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    CreatedAt = DateTime.UtcNow
                });
                return true;
            }
        }

        public Task<Result> UnfollowAsync(int followerId, int followingId)
        {
            throw new NotImplementedException();
        }
    }
}
