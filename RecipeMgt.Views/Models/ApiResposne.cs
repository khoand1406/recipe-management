namespace RecipeMgt.Views.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public IReadOnlyList<string>? Errors { get; init; }
        public string? ErrorCode { get; init; }

        public int? StatusCode { get; init; }
        public string? RequestId { get; init; }
        public T? Data { get; init; }

        public static ApiResponse<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Message = message,
            Data = data,

        };

        public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null, string? errorCode = null, int? statusCode = null, string? requestId = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList(),
            ErrorCode = errorCode,
            StatusCode = statusCode,
            RequestId = requestId


        };
    }

}

    

