namespace sevaLK_service_auth.Auth.Login;

public record LoginRequest(
    string Email,
    string Password
);
