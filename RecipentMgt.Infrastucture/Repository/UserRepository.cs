using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository
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
            var user= await getUserByEmail(email);
            Console.WriteLine(user);
            return user != null;
        }

        public async Task<(bool Success, string Message, int CarriageId)> createUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return (true, "User created successfully", user.UserId);
        }

        public async Task<bool> deleteUser(int userId)
        {
            var userFound= await _context.Users.FindAsync(userId);
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

        public async Task<User> getUserAsync(int userId)
        {
            var userFound= await (_context.Users.FindAsync(userId));
            return userFound ?? new User();
        }

        public async Task<User> getUserByEmail(string email)
        {
            var user= await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
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
    }
}
