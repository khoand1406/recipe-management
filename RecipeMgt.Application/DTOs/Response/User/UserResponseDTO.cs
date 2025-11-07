using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.User
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? AvatarUrl { get; set; }
        
    }
}
