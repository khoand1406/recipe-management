using AutoMapper;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Following;
using RecipentMgt.Infrastucture.Repository.Ratings;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IFollowingRepository _followRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(
            IFollowingRepository followRepo,
            IRatingRepository ratingRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _followRepo = followRepo;
            _ratingRepo = ratingRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        
        public async Task<bool> ToggleFollowAsync(int followerId, int followingId)
        {
            if (await _followRepo.IsFollowingAsync(followerId, followingId))
            {
                await _followRepo.UnfollowAsync(followerId, followingId);
                return false;
            }
            else
            {
                await _followRepo.FollowAsync(new Following
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    CreatedAt = DateTime.UtcNow
                });
                return true;
            }
        }

        public Task<bool> IsFollowingAsync(int followerId, int followingId)
            => _followRepo.IsFollowingAsync(followerId, followingId);


        async Task<List<UserResponseDTO>> IUserService.GetFollowersAsync(int userId)
        {
            var following = await _followRepo.GetFollowers(userId);
            return _mapper.Map<List<UserResponseDTO>>(following);
        }

        async Task<List<UserResponseDTO>> IUserService.GetFollowingAsync(int userId)
        {
            var follows= await _followRepo.GetFollowingUsers(userId);
            return _mapper.Map<List<UserResponseDTO>>(follows);    
        }

        public async Task<List<UserResponseDTO>> GetTopContributors()
        {
            var topContributors = await _userRepo.GetTopContributors();
            return _mapper.Map<List<UserResponseDTO>>(topContributors);
        }
    }

}
