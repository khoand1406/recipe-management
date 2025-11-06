using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Followings
{
    public interface IFollowingService
    {
        Task<bool> ToggleFollowAsync(int followerId, int followingId);
    }
}

