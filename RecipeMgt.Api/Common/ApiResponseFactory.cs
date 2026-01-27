using FluentValidation.Results;
using RecipeMgt.Application.DTOs;


namespace RecipeMgt.Api.Common
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(T data, HttpContext httpContext) => new()
        {
            Success = true,
            Data = data,
            RequestId = httpContext.TraceIdentifier

        };

        public static ApiResponse Success(string message, HttpContext httpContext) => new() { Success = true, RequestId = httpContext.TraceIdentifier, Message = message };

        public static ApiResponse Fail(
        ValidationResult validation,
        HttpContext ctx) =>
        new()
        {
            Success = false,
            Message = "Validation failed",
            Errors = validation.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList(),
            RequestId = ctx.TraceIdentifier
        };

        public static ApiResponse<T> Fail<T>(
            ValidationResult validation,
            HttpContext ctx) =>
            new()
            {
                Success = false,
                Message = "Validation failed",
                Errors = validation.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList(),
                RequestId = ctx.TraceIdentifier
            };
        public static ApiResponse Fail(
            string message,
            HttpContext ctx,
            List<string>? errors = null) =>
            new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                RequestId = ctx.TraceIdentifier
            };

        public static ApiResponse<T> Fail<T>(
            string message,
            HttpContext ctx,
            List<string>? errors = null) =>
            new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                RequestId = ctx.TraceIdentifier
            };
    }
}

