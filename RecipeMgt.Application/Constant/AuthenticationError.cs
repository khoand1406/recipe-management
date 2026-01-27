using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Constant
{
    public static class AuthenticationError
    {
        public const string AuthenError = "AUTHENTICATION_FAILED";
        public const string BadCredentials = "BAD_CREDENTIALS";
        public const string DuplicateEmail = "DUPLICATE_EMAIL";
        public const string DuplicateUsername = "DUPLICATE_USERNAME";
        public const string InvalidToken = "INVALID_TOKEN";
        public const string TokenExpired = "TOKEN_EXPIRED";
    }
}
