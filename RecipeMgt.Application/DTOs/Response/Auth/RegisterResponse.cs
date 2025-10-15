using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Auth
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public UserResponse? User { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
