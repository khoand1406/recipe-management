using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Domain.RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);

        Task<User?> GetByEmailAsync(string email);

        Task<List<User>> GetUsersByIds(List<int> items);
        Task<User?> GetByUserName(string username);

        Task<bool> ExistsByEmailAsync(string email);

        Task<int> CountAsync();

        Task<PagedResponse<User>> GetUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            int? userStatus);

        Task<List<User>> GetTopContributorsAsync();

        Task<int> CreateAsync(User user);

        Task CreateBatchAsync(List<User> users);
        Task UpdateAsync(User user);

        Task UpdateRangeAsync(List<User> users);

        Task DeleteAsync(User user);

        Task CreateUserActivityLogAsync(
            int? userId,
            string? sessionId,
            UserActivityType action,
            string target,
            int targetId,
            string? description);
        Task<User> UpsertGoogleUserAsync(
            string providerId,
            string email,
            string username);

        Task<User> UpsertAzureUserAsync(
            string email,
            string? name);
        Task<Role?> GetRoleByName(string roleName);
        Task<IEnumerable<string>> GetExistingEmails(List<string> emails);
        Task <IEnumerable<string>>GetExistingName(List<string> names);
    }
}
