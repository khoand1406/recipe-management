using RecipeMgt.Application.DTOs.Response.Auth;
using RecipeMgt.Application.DTOs.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Users
{
    public interface IUserService
    {
        Task<bool> ToggleFollowAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<List<UserResponseDTO>> GetFollowersAsync(int userId);
        Task<List<UserResponseDTO>> GetFollowingAsync(int userId);
        Task<List<UserResponseDTO>> GetTopContributors();

        // RATING (User - Recipe)

    }
}
