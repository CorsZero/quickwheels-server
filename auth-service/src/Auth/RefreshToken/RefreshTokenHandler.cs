using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;
using sevaLK_service_auth.Shared.Helpers;

namespace sevaLK_service_auth.Auth.RefreshToken;

public class RefreshTokenHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<ApiResponse> Handle(string? refreshTokenValue, HttpResponse httpResponse)
    {
        if (string.IsNullOrEmpty(refreshTokenValue))
        {
            return ApiResponse.ErrorResult("No refresh token provided");
        }

        // Find user by refresh token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == refreshTokenValue);

        if (user == null || !user.IsRefreshTokenValid())
        {
            CookieHelper.ClearAuthCookies(httpResponse);
            return ApiResponse.ErrorResult("Invalid or expired refresh token");
        }

        if (!user.IsActive)
        {
            CookieHelper.ClearAuthCookies(httpResponse);
            return ApiResponse.ErrorResult("Account is deactivated");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var accessExpiryMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
        
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
        await _userRepository.UpdateAsync(user);

        // Set new tokens as HttpOnly cookies
        CookieHelper.SetAuthCookies(
            httpResponse,
            accessToken,
            refreshToken,
            accessExpiryMinutes,
            refreshTokenExpiryDays,
            _environment.IsDevelopment()
        );

        return ApiResponse.SuccessResult(message: "Token refreshed successfully");
    }
}
