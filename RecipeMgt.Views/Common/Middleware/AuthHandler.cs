using System.Net.Http.Headers;

namespace RecipeMgt.Views.Common.Middleware
{
    public class AuthHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(IHttpContextAccessor contextAccessor, ILogger<AuthHandler> logger)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _contextAccessor.HttpContext?
                .Session.GetString("JwtToken");
            _logger.LogInformation($"Tokennnnnnnnnnnnnnn:{token}");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

