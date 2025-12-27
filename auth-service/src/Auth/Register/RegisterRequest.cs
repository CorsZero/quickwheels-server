namespace sevaLK_service_auth.Auth.Register;

public record RegisterRequest(
    string Email,
    string FullName,
    string Phone,
    string Password,
    string ConfirmPassword,
    string Role // "User" or "Admin"
);
