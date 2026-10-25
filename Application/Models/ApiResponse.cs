namespace Application.Models;

public sealed class ApiResponse<T>
{
    public int Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string message) => new()
    {
        Status = 200,
        Message = message,
        Data = data
    };

    public static ApiResponse<object?> Failure(int status, string message) => new()
    {
        Status = status,
        Message = message,
        Data = null
    };
}
