namespace sevaLK_service_auth.Auth.Register;

public record RegisterRequest(
    string Email,
    string Name,
    string Password,
    string ConfirmPassword,
    string Role // "Customer" or "Provider"
);
