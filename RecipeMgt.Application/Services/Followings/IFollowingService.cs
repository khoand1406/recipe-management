using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Follows;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Followings
{
    public interface IFollowingService
    {
        Task<bool> ToggleFollowAsync(int followerId, int followingId);

        Task<Result> FollowAsync(CreateFollowDTO follow);
        Task <Result>UnfollowAsync(int followerId, int followingId);

        Task<Result<List<UserResponseDTO>>> GetFollowers(int userId);

        Task<Result<List<UserResponseDTO>>> GetFollowingUsers(int userId);
    }
}

