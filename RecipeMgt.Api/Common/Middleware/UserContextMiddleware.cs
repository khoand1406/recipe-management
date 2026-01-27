using System.Security.Claims;

namespace RecipeMgt.Api.Common.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<UserContextMiddleware> _logger;

        public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.User.FindFirst("role")?.Value;
                if (userId != null)
                {
                    context.Items["UserId"] = int.Parse(userId);
                    context.Items["Role"] = role;
                    _logger.LogDebug("UserContextMiddleware: UserId={UserId}, Role={Role}", userId, role);
                }
            }
            await _next(context);
        }


    }
}
