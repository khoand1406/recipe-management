using Microsoft.AspNetCore.Http;

namespace RecipeMgt.Application.Exceptions
{
    public abstract class AppException: Exception
    {
        protected AppException(string message): base(message) { }
        public abstract int StatusCode { get; }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
        public override int StatusCode => StatusCodes.Status404NotFound;
    }

    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message) { }
        public override int StatusCode => StatusCodes.Status403Forbidden;
    }

    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message) { }
        public override int StatusCode => StatusCodes.Status400BadRequest;
    }

    public sealed class AuthenticationException : AppException
    {
        public AuthenticationException(string message) : base(message)
        {
        }

        public override int StatusCode => StatusCodes.Status401Unauthorized;
    }

    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public override int StatusCode => StatusCodes.Status401Unauthorized;
    }


}
