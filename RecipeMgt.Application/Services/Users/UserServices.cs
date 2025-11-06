using RecipeMgt.Application.DTOs.Response.User;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Users
{
    public class UserServices:IUserService
    {
        private readonly IUserRepository _repository;


        public UserServices(IUserRepository repository)
        {
            _repository = repository;
        }

        
        public Task<bool> IsFollowingAsync(int followerId, int followingId)
        {
            throw new NotImplementedException();
        }

        
        public Task<bool> ToggleFollowAsync(int followerId, int followingId)
        {
            throw new NotImplementedException();
        }

        Task<List<UserResponseDTO>> IUserService.GetFollowersAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task<List<UserResponseDTO>> IUserService.GetFollowingAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
