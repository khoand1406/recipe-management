using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeMgt.Domain.Enums;

namespace RecipeMgt.Domain.Entities
{
    public class UserActivityLog
    {
        public int ActivityLogId { get; set; }

        public int? UserId { get; set; }

        public string? SessionId { get; set; }

        public UserActivityType ActivityType { get; set; }

        public string ?TargetType { get; set; }
        public int? TargetId { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
