using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public UserResponse? User { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }

    
}
