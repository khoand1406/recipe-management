using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Auth
{
    public class OAuthService : IOAuthService
    {
        private IConfiguration _config;
        private ILogger<OAuthService> _logger;

        public OAuthService(IConfiguration config, ILogger<OAuthService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyAsync(string IdToken)
        {
            return await GoogleJsonWebSignature.ValidateAsync(IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience= new[] { _config["Google:ClientId"] }});
        }
    }
}
