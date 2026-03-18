using RecipeMgt.Application.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.User
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UserResponseMgtDTO
    {
        public int UserId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? RoleName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserStatus UserStatus { get; set; }

        public string? Provider { get; set; }

        public int TotalRecipes { get; set; }

        public int TotalFollowers { get; set; }
    }

    public class UserDetailResponse
    {
        public int UserId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? RoleName { get; set; }

        public string? Provider { get; set; }

        public string? ProviderId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserStatus Status { get; set; }

        public int TotalRecipes { get; set; }

        public int TotalRatings { get; set; }

        public int TotalFollowers { get; set; }

        public int TotalFollowing { get; set; }
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "FULLNAME_REQUIRED")]
        [MaxLength(100, ErrorMessage = "FULLNAME_TOO_LONG")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "EMAIL_REQUIRED")]
        [EmailAddress(ErrorMessage = "WRONG_EMAIL_FORMAT")]
        [MaxLength(100, ErrorMessage = "EMAIL_TOO_LONG")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "PASSWORD_REQUIRED")]
        [MinLength(6, ErrorMessage = "PASSWORD_TOO_SHORT")]
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; } = RoleConstants.USER_ROLE_ID;

        public bool IsActived { get; set; } = true;
    }

    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public int? RoleId { get; set; }

        public bool? IsActived { get; set; }

        public bool? IsBanned { get; set; }
    }

    public class BatchUserRequest<T>
    {
        public List<T> Items { get; set; } = new();
    }

    public class BatchUserIdsRequest
    {
        public List<int> Items { get; set; } = new();
    }

}
