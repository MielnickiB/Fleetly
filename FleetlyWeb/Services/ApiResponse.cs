namespace FleetlyWeb.Services;

public sealed record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public string? Error { get; init; }

    public static ApiResponse<T> SuccessResult(T? data, int status) =>
        new() { Success = true, Data = data, StatusCode = status };

    public static ApiResponse<T> ErrorResult(string? error, int status) =>
        new() { Success = false, Data = default, StatusCode = status, Error = error };
}
