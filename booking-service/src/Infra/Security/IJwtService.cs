namespace booking_service.Infra.Security;

public interface IJwtService
{
    (bool isValid, Guid userId, string? role) ValidateToken(string token);
}
