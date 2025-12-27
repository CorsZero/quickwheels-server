namespace vehicle_service.Shared.Types;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public object? Error { get; set; }

    public static ApiResponse SuccessResult(object? data = null, string message = "Request successful")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data,
            Error = null
        };
    }

    public static ApiResponse ErrorResult(string message, object? error = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Error = error ?? new List<string>()
        };
    }
}
