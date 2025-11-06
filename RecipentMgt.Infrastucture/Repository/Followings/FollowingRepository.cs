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
    public class FollowingRepository:IFollowingRepository
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
    }
}
