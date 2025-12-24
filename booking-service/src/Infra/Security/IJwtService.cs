using sevaLK_service_auth.Domain.Entities;

namespace sevaLK_service_auth.Infra.Security;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    (bool isValid, Guid userId, string? role) ValidateToken(string token);
}
