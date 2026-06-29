namespace Shared.Models;

/// <summary>
/// Standard API response wrapper used across all services.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string? StackTrace { get; set; }
    public string? InternalError { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Fail(string message, Exception? ex = null) => new()
    {
        Success = false,
        Message = message,
        StackTrace = ex?.StackTrace,
        InternalError = ex?.ToString()
    };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    public new static ApiResponse Fail(string message, Exception? ex = null) => new()
    {
        Success = false,
        Message = message,
        StackTrace = ex?.StackTrace,
        InternalError = ex?.ToString()
    };
}
