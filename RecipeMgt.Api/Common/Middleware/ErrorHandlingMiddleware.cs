using FluentValidation;
using FluentValidation.Results;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace RecipeMgt.Api.Common.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = ApiResponseFactory.Fail(
                    new ValidationResult(ex.Errors),
                    context
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled Application Exception");
                context.Response.StatusCode= ex.StatusCode;
                context.Response.ContentType = "application/json";
                var response = ApiResponseFactory.Fail(ex.Message, context);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponseFactory.Fail(
                    "Internal server error",
                    context,
                    [ex.Message]
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            }
        }

        
    }
}
