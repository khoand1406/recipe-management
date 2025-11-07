using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class Following
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public User Follower { get; set; }

        public int FollowingId { get; set; }
        public User FollowingUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
