using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Application.Services.Management.User;
using RecipeMgt.Domain.Enums;

namespace RecipeMgt.Api.Controllers.Management
{
    [Authorize(Policy ="AdminOnly")]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        public UserController(IUserManagementService userManagementService) { 
            _userManagementService = userManagementService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetListUsers(string? searchQuery, UserStatus? userStatus, int page = 1, int pageSize = 10)
        {
            var usersData = await _userManagementService.GetUsers(page, pageSize, searchQuery, userStatus);
            return Ok(ApiResponseFactory.Success(usersData.Value, HttpContext));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var userData = await _userManagementService.GetUserDetail(id);
            return Ok(ApiResponseFactory.Success(userData.Value, HttpContext));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var userData = await _userManagementService.CreateUser(request);

            return Ok(ApiResponseFactory.Success(userData.Value, HttpContext));

        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var statisticsData = await _userManagementService.GetUsersStatistic();
            return Ok(ApiResponseFactory.Success(statisticsData.Value, HttpContext));
        }

        [HttpPost("create-batch")]
        public async Task<IActionResult> CreateBatch([FromBody] BatchUserRequest<CreateUserRequest> request)
        {
            var result = await _userManagementService.CreateBatchUsers(request);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> CreateFromCsv(IFormFile file)
        {
            var result = await _userManagementService.CreateUsersFromCsv(file);

            
            if (result.SuccessCount == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(
                    "IMPORT_FAILED",
                    
                    HttpContext
                ));
            }
            if (result.FailedCount > 0)
            {
                return Ok(ApiResponseFactory.Success(
                    result,
                    HttpContext
                    
                ));
            }
            return Ok(ApiResponseFactory.Success(
                result,
                HttpContext
                
            ));
        }


        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request, int id)
        {
            var result = await _userManagementService.UpdateUser(request, id);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPost("update-batch")]
        public async Task<IActionResult> UpdateBatch([FromBody] BatchUserRequest<UpdateUserRequest> request)
        {
            var result = await _userManagementService.UpdateBatchUsers(request);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPatch("{id}/deactive")]
        public async Task<IActionResult> Deactive(int id)
        {
            var result = await _userManagementService.DeactiveUser(id);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPost("deactive-batch")]
        public async Task<IActionResult> DeactiveBatch([FromBody] BatchUserIdsRequest request)
        {
            var result = await _userManagementService.DeActiveBatchUser(request);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPatch("{id}/ban")]
        public async Task<IActionResult> Ban(int id)
        {
            var result = await _userManagementService.BanUser(id);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPatch("ban/batch-user")]
        public async Task<IActionResult> BanBatch([FromBody] BatchUserIdsRequest request)
        {
            var result = await _userManagementService.BanBatchUser(request);
            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpPatch("{id}/recover")]
        public async Task<IActionResult> RecoverAccount(int id)
        {
            var result = await _userManagementService.RecoverUserAccount(id);
            return Ok(ApiResponseFactory.Success(result.Value, httpContext: HttpContext));
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userManagementService.DeleteUser(id);

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }

        [HttpDelete("users/batch")]
        public async Task<IActionResult> DeleteBatch([FromBody] BatchUserIdsRequest request)
        {
            var result = await _userManagementService.DeleteBatchUsers(request);

            return Ok(ApiResponseFactory.Success(result.Value, HttpContext));
        }
    }
}
