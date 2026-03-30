using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Auth
{
    public class GoogleLoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; }= string.Empty;
    }

    public class AzureLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
