using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public interface IOAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyAsync(string IdToken);
    }
}
