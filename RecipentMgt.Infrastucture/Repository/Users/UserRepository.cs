using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly RecipeManagementContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(RecipeManagementContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> checkDuplicateEmail(string email)
        {
            var user = await getUserByEmail(email);
            Console.WriteLine(user);
            return user != null;
        }

        public async Task<(bool Success, string Message, int CarriageId)> createUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return (true, "User created successfully", user.UserId);
        }

        public async Task CreateUserActivityLog(int ?userId, string? sessionId,  UserActivityType action, string target, int targetId, string? description)
        {
            await _context.UserActivityLogs.AddAsync(new UserActivityLog {
                UserId= userId,
                SessionId= sessionId,
                ActivityType= action,
                CreatedAt= DateTime.Now,
                Description= description,
                TargetId= targetId,
                TargetType= target,
            });
            await _context.SaveChangesAsync();  
        }

        public async Task<bool> deleteUser(int userId)
        {
            var userFound = await _context.Users.FindAsync(userId);
            if (userFound != null)
            {
                _context.Users.Remove(userFound);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<User>> GetTopContributors()
        {
            var topContributor= await _context.Users
                .Include(u=> u.UserStatistic)
                .OrderBy(u=> u.UserStatistic.RecipeCount)
                .Take(5)
                .ToListAsync();
            return topContributor;
        }


        public async Task<User> getUserAsync(int userId)
        {
            var userFound = await _context.Users.FindAsync(userId);
            return userFound ?? new User();
        }

        public async Task<User> getUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            return user ?? null;
        }

        public async Task<User?> getUserByUsername(string username)
        {
            return await _context.Users
        .FirstOrDefaultAsync(x => x.FullName == username);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<(bool Success, string Message, int UserId)> updateUser(User user)
        {
            var userFound = await _context.Users.FindAsync(user.UserId);
            if (userFound != null)
            {
                userFound.FullName = user.FullName;
                userFound.Email = user.Email;
                await _context.SaveChangesAsync();
                return (true, "Update userinfo successfully", userFound.UserId);

            }
            else
            {
                return (false, "User not found", 0);
            }
        }

        public async Task<User> UpsertGoogleUserAsync(string providerId, string email, string username, string avatar)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
            x.Provider == "Google" &&
            x.ProviderId == providerId);
            if(user != null)
            {
                return user;
            }
            user = new User
            {
                Email= email,
                FullName = username,
                Provider= "Google",
                ProviderId= providerId,
                CreatedAt= DateTime.Now,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
