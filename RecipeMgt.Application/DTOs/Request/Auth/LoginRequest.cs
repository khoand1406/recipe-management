using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Request.Auth
{
    public class LoginRequest
    {

        public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
    }
}
