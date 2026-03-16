using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Application.Exceptions;
using RecipentMgt.Infrastucture.Repository.Following;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Management.User
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowingRepository _followingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(IUserRepository userRepository, IMapper mapper, ILogger<UserManagementService> logger, IFollowingRepository followingRepository)
        {
            _userRepository = userRepository;
            _followingRepository = followingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result<bool>> BanBatchUser(BatchUserIdsRequest batchUserIds)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserBasicResponse>> BanUser(int id)
        {
            var user= await _userRepository.getUserAsync(id);
            if (user == null)
            {
                throw new NotFoundException("USER_NOT_FOUND");
            }
            await _userRepository.BanUser(user);
            var mappedUser= _mapper.Map<UserBasicResponse>(user);
            return Result<UserBasicResponse>.Success(mappedUser);
        }

        public Task<Result<bool>> CreateBatchUsers(BatchUserRequest<CreateUserRequest> userRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserResponseDTO>> CreateUser(CreateUserRequest request)
        {
            var payload = _mapper.Map<Domain.Entities.User>(request);
            var result = await _userRepository.createUser(payload);
            var createdUser = await _userRepository.getUserAsync(result.CarriageId);
            if(createdUser == null)
            {
                return Result<UserResponseDTO>.Failure("FAIL TO CREATE NEW USER");
            }
            var mappedResponse= _mapper.Map<UserResponseDTO>(createdUser);
            return Result<UserResponseDTO>.Success(mappedResponse);
            
        }

        public Task<Result<bool>> DeActiveBatchUser(BatchUserIdsRequest batchUserIds)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeactiveUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteBatchUsers(BatchUserIdsRequest batchUserIds)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserResponseDTO>> DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserDetailResponse>> GetUserDetail(int id)
        {
            var userDetail= await _userRepository.getUserAsync(id);
            if(userDetail == null)
            {
                throw new NotFoundException("NOT_FOUND_USER");
            }
            var totalFollowers = await _followingRepository.GetFollowers(userDetail.UserId);
            var totalFollowing= await _followingRepository.GetFollowingUsers(userDetail.UserId);
            var mappedResult = new UserDetailResponse
            {
                UserId = userDetail.UserId,
                Email = userDetail.Email,
                FullName = userDetail.FullName,
                CreatedAt = userDetail.CreatedAt,
                ProviderId = userDetail.ProviderId,
                Provider = userDetail.Provider,
                RoleName = userDetail.Role.RoleName,
                Status = ConvertFromUserDetail(userDetail),
                
                TotalFollowers = totalFollowers.Count,
                TotalFollowing= totalFollowing.Count,
                TotalRatings= userDetail.Ratings.Count,
                TotalRecipes= userDetail.Recipes.Count,
            };
            return Result<UserDetailResponse>.Success(mappedResult);
        }

        public async Task<Result<PagedResponse<UserResponseMgtDTO>>> GetUsers(int page, int pageSize, string? searchQuery, UserStatus? userStatus)
        {
            var userQuery = await _userRepository.GetUsersAsync(page, pageSize, searchQuery, (int?)userStatus);
            var mappedResult= userQuery.Items.Select(x=> _mapper.Map<UserResponseMgtDTO>(x)).ToList();
            var result= new PagedResponse<UserResponseMgtDTO> { 
                Items= mappedResult,
                Page= userQuery.Page,
                PageSize= userQuery.PageSize,
                TotalItems= userQuery.TotalItems,
                TotalPages= userQuery.TotalPages,

            };
            return Result<PagedResponse<UserResponseMgtDTO>>.Success(result);
        }

        public Task<Result<bool>> RecoverUserAccount(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateBatchUsers(BatchUserRequest<UpdateUserRequest> userRequests)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserResponseDTO>> UpdateUser(UpdateUserRequest request, int id)
        {
            var payload = _mapper.Map<Domain.Entities.User>(request);
            var (Success, Message, UserId) = await _userRepository.updateUser(payload, id);
            if (Success)
            {
                var userUpdated= await _userRepository.getUserAsync(UserId);
                var mappedResponse= _mapper.Map<UserResponseDTO>(userUpdated);
                return Result<UserResponseDTO>.Success(mappedResponse);
            }
            return Result<UserResponseDTO>.Failure(Message);

        }

        private UserStatus ConvertFromUserDetail(Domain.Entities.User user)
        {
            if (user.DeleteAt != null)
                return UserStatus.Deleted;

            if (user.IsBanned)
                return UserStatus.Banned;

            if (!user.IsActived)
                return UserStatus.Deactivated;

            return UserStatus.Active;
        }
    }
}
