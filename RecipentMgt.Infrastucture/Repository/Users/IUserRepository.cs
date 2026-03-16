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
        Task<User?> getUserByEmail(string email);

        Task<User?> getUserByUsername(string username);

        Task<PagedResponse<User>> GetUsersAsync(int page, int pageSize, string? searchQuery, int? userStatus);

        Task<User?> getUserAsync(int userId);

        Task<(bool Success, string Message, int CarriageId)> createUser(User user);
        Task<(bool Success, string Message, int UserId)> updateUser(User user, int id);

        Task<bool> deleteUser(int userId);
        Task<bool> checkDuplicateEmail(string email);

        public Task<User> UpsertGoogleUserAsync(string providerId, string email, string username, string avatar);

        public Task CreateUserActivityLog(int? userId,string? sessionId, UserActivityType action, string target, int targetId, string? description);
        Task<List<User>> GetTopContributors();
        Task<User> UpsertAzureUserAsync(string email, string? name);
        Task<int> CountAsync();
        Task BanUser(User user);

        Task UnbanUser(User user);
    }
}
