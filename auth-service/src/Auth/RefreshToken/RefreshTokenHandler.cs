using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.RefreshToken;

public class RefreshTokenHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<ApiResponse> Handle(RefreshTokenRequest request)
    {
        // Find user by refresh token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

        if (user == null || !user.IsRefreshTokenValid())
        {
            return ApiResponse.ErrorResult("Invalid or expired refresh token");
        }

        if (!user.IsActive)
        {
            return ApiResponse.ErrorResult("Account is deactivated");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        var refreshTokenExpiryDays = int.Parse(_configuration["RefreshTokenExpirationDays"] ?? "7");
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = int.Parse(_configuration["ExpirationMinutes"] ?? "60") * 60
        }, "Token refreshed successfully");
    }
}
