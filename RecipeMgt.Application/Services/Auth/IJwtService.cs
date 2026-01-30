using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public interface IJwtService
    {
        public string GenerateJWTToken(User user);
    }
}
