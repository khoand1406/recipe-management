using Microsoft.AspNetCore.Http;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Management.User
{
    public interface IUserManagementService
    {
        Task<Result<PagedResponse<UserResponseMgtDTO>>> GetUsers(int page, int pageSize, string? searchQuery, UserStatus? userStatus);

        Task<Result<UserDetailResponse>> GetUserDetail(int id);

        Task<Result<UserResponseDTO>> CreateUser(CreateUserRequest request);

        Task<Result<UserResponseDTO>> UpdateUser(UpdateUserRequest request, int id);

        Task<Result<UserResponseDTO>> DeleteUser(int id);

        Task<Result<BatchImportResult>> CreateBatchUsers(BatchUserRequest<CreateUserRequest> userRequest);

        Task<Result<bool>> UpdateBatchUsers(BatchUserRequest<UpdateUserRequest> userRequests);

        Task<Result<bool>> DeleteBatchUsers(BatchUserIdsRequest batchUserIds);

        Task<Result<bool>> DeactiveUser(int id);

        Task<Result<bool>> DeActiveBatchUser(BatchUserIdsRequest batchUserIds);

        Task<Result<UserBasicResponse>> BanUser(int id);

        Task<Result<bool>> BanBatchUser(BatchUserIdsRequest batchUserIds);

        Task<Result<bool>> RecoverUserAccount(int id);

        Task<Result<UsersStatistic>> GetUsersStatistic();
        Task<BatchImportResult> CreateUsersFromCsv(IFormFile file);
    }
}
