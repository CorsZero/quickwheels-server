namespace booking_service.Shared.Types;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponse Success(object data)
    {
        return new ApiResponse { Success = true, Data = data };
    }

    public static ApiResponse Error(string error)
    {
        return new ApiResponse { Success = false, Error = error };
    }
}
