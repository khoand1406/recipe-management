using AutoMapper;
using Azure.Core;
using CsvHelper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Application.Exceptions;
using RecipeMgt.Application.Utils;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Following;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IValidator<CreateUserCsvDto> _createUserCsvValidator;

        public UserManagementService(IUserRepository userRepository, IMapper mapper, ILogger<UserManagementService> logger, IFollowingRepository followingRepository, IValidator<CreateUserCsvDto> validator)
        {
            _userRepository = userRepository;
            _followingRepository = followingRepository;
            _mapper = mapper;
            _logger = logger;
            _createUserCsvValidator = validator;
        }

        public async Task<Result<bool>> BanBatchUser(BatchUserIdsRequest batchUserIds)
        {
            var users = await _userRepository.GetUsersByIds(batchUserIds.Items);

            foreach (var user in users)
            {
                user.IsBanned = true;
            }

            await _userRepository.UpdateRangeAsync(users);

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserBasicResponse>> BanUser(int id)
        {
            var user= await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("USER_NOT_FOUND");
            }
            user.IsBanned = true;
            await _userRepository.UpdateAsync(user);
            var mappedUser= _mapper.Map<UserBasicResponse>(user);
            return Result<UserBasicResponse>.Success(mappedUser);
        }

        public async Task<Result<BatchImportResult>> CreateBatchUsers(BatchUserRequest<CreateUserRequest> request)
        {
            var result = new BatchImportResult();
            var users = new List<Domain.Entities.User>();

            foreach (var item in request.Items)
            {
                try
                {
                    if (await _userRepository.GetByEmailAsync(item.Email)!= null)
                        throw new Exception("Email already exists");

                    if (await _userRepository.GetByUserName(item.FullName) != null)
                        throw new Exception("Username already exists");

                    var user = _mapper.Map<Domain.Entities.User>(item);

                    user.PasswordHash = UserUtils.HashPassword(item.Password);
                    
                    user.CreatedAt = DateTime.UtcNow;
                    user.IsActived = true;

                    users.Add(user);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"{item.Email}: {ex.Message}");
                }
            }

            if (users.Count != 0)
                await _userRepository.CreateBatchAsync(users);

            return Result<BatchImportResult>.Success(result);
        }

        public async Task<Result<UserResponseDTO>> CreateUser(CreateUserRequest request)
        {
            var user = _mapper.Map<Domain.Entities.User>(request);
            user.PasswordHash = UserUtils.HashPassword(request.Password);

            user.CreatedAt = DateTime.UtcNow;
            user.IsActived = true;
            var result = await _userRepository.CreateAsync(user);
            var createdUser = await _userRepository.GetByIdAsync(result);
            if(createdUser == null)
            {
                return Result<UserResponseDTO>.Failure("FAIL TO CREATE NEW USER");
            }
            var mappedResponse= _mapper.Map<UserResponseDTO>(createdUser);
            return Result<UserResponseDTO>.Success(mappedResponse);
            
        }

        public async Task<BatchImportResult> CreateUsersFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("FILE_EMPTY");

            if (file.Length > 5 * 1024 * 1024)
                throw new BadRequestException("FILE_TOO_LARGE");

            var result = new BatchImportResult();
            var users = new List<Domain.Entities.User>();

            using var stream = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<CreateUserCsvDto>().ToList();

            var emails = records
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .Select(x => x.Email)
                .ToList();

            var names = records.Where(x => !string.IsNullOrEmpty(x.FullName)).Select(x=> x.FullName).ToList();


            var existingEmails = await _userRepository.GetExistingEmails(emails);
            var existingUserName = await _userRepository.GetExistingName(names);
            var existingEmailSet = new HashSet<string>(existingEmails);
            var existingUsernameSet = new HashSet<string>(existingUserName);

            int row = 1;

            foreach (var record in records)
            {
                try
                {
                    var validationResult= await _createUserCsvValidator.ValidateAsync(record);

                    if (!validationResult.IsValid)
                    {
                        var errors= validationResult.Errors.Select(x => x.ErrorMessage);
                        throw new BadRequestException(string.Join(", ", errors));
                    }

                    if (existingEmailSet.Contains(record.Email))
                        throw new BadRequestException("EMAIL_ALREADY_EXISTS");

                    if (existingUsernameSet.Contains(record.FullName)) 
                        throw new BadRequestException("USERNAME_ALREADY_EXISTS");

                    var roleId = await MapRole(record.Role);

                    var user = new Domain.Entities.User
                    {
                        Email = record.Email,
                        FullName = record.FullName,
                        PasswordHash = UserUtils.HashPassword(record.Password),
                        RoleId = roleId,
                        CreatedAt = DateTime.UtcNow,
                        IsActived = true,
                        IsBanned = false
                    };

                    users.Add(user);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Row {row} ({record.Email}): {ex.Message}");
                }

                row++;
            }

            if (users.Count != 0)
                await _userRepository.CreateBatchAsync(users);

            return result;
        }

        public async Task<Result<bool>> DeActiveBatchUser(BatchUserIdsRequest batchUserIds)
        {
            var users = await _userRepository.GetUsersByIds(batchUserIds.Items);

            foreach (var user in users)
            {
                user.IsActived = false;
            }

            await _userRepository.UpdateRangeAsync(users);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeactiveUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("USER_NOT_FOUND");
            user.IsActived = false;
            await _userRepository.UpdateAsync(user);
           
            return Result<bool>.Success(true);
            
        }

        public async Task<Result<bool>> DeleteBatchUsers(BatchUserIdsRequest batchUserIds)
        {
            var users = await _userRepository.GetUsersByIds(batchUserIds.Items);

            foreach (var user in users)
            {
                user.DeleteAt = DateTime.UtcNow;
            }

            await _userRepository.UpdateRangeAsync(users);

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserResponseDTO>> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("USER_NOT_FOUND");
            user.DeleteAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation($"Successfully Delete User with id: {id}");
            return Result<UserResponseDTO>.Success(_mapper.Map<UserResponseDTO>(user));
        }

        public async Task<Result<UserDetailResponse>> GetUserDetail(int id)
        {
            var userDetail= await _userRepository.GetByIdAsync(id);
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

        public async Task<Result<bool>> RecoverUserAccount(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND");

            user.DeleteAt = null;
            user.IsBanned = false;
            user.IsActived = true;
            await _userRepository.UpdateAsync(user);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateBatchUsers(BatchUserRequest<UpdateUserRequest> userRequests)
        {
            var users = await _userRepository.GetUsersByIds(userRequests.Items.Select(x => x.UserId).ToList());

            foreach (var user in users)
            {
                var updateData = userRequests.Items.First(x => x.UserId == user.UserId);
                _mapper.Map(updateData, user);
            }

            await _userRepository.UpdateRangeAsync(users);

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserResponseDTO>> UpdateUser(UpdateUserRequest request, int id)
        {
            var payload = _mapper.Map<Domain.Entities.User>(request);
            await _userRepository.UpdateAsync(payload);

            var userUpdated = await _userRepository.GetByIdAsync(id);
            var mappedResponse = _mapper.Map<UserResponseDTO>(userUpdated);
            return Result<UserResponseDTO>.Success(mappedResponse);
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

        private async Task<int> MapRole(string roleName)
        {
            var role = await _userRepository.GetRoleByName(roleName);
            return role?.RoleId ?? RoleConstants.USER_ROLE_ID;
        }
    }
}
