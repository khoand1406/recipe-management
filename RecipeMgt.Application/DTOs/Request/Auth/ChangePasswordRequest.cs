using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Auth
{
    public class ChangePasswordRequest
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Old password is required.")]
        [MinLength(6, ErrorMessage = "Old password must be at least 6 characters.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "New password must contain uppercase, lowercase, digit, and special character.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Confirm password does not match new password.")]
        public string ConfirmPassword { get; set; } = string.Empty;


    }
}
