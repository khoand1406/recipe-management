﻿using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Users
{
    public interface IUserRepository
    {
        Task<User> getUserByEmail(string email);

        Task<User?> getUserByUsername(string username);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> getUserAsync(int userId);

        Task<(bool Success, string Message, int CarriageId)> createUser(User user);
        Task<(bool Success, string Message, int UserId)> updateUser(User user);

        Task<bool> deleteUser(int userId);
        Task<bool> checkDuplicateEmail(string email);
    }
}
