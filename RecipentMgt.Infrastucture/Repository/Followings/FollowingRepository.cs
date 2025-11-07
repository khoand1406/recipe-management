using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Following
{
    public class FollowingRepository : IFollowingRepository
    {
        private readonly RecipeManagementContext _context;

        public FollowingRepository(RecipeManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followingId)
       => await _context.Following
           .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        public async Task FollowAsync(RecipeMgt.Domain.Entities.Following follow)
        {
            await _context.Following.AddAsync(follow);
            await _context.SaveChangesAsync();
        }

        public async Task UnfollowAsync(int followerId, int followingId)
        {
            var follow = await _context.Following
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
            if (follow != null)
            {
                _context.Following.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetFollowers(int userId)
        {
            // Lấy danh sách người đang FOLLOW user này
            var followers = await _context.Following
                .Where(f => f.FollowingId == userId)
                .Select(f => f.Follower)
                .ToListAsync();

            return followers;
        }

        public async Task<List<User>> GetFollowingUsers(int userId)
        {
            // Lấy danh sách user mà user này đang FOLLOW
            var following = await _context.Following
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingUser)
                .ToListAsync();

            return following;
        }
    }
}
