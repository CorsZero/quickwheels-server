using Microsoft.AspNetCore.Http;

namespace sevaLK_service_auth.Shared.Helpers;

public static class CookieHelper
{
    private const string AccessTokenCookie = "access_token";
    private const string RefreshTokenCookie = "refresh_token";

    /// <summary>
    /// Sets both access and refresh tokens as HttpOnly secure cookies
    /// </summary>
    public static void SetAuthCookies(
        HttpResponse response,
        string accessToken,
        string refreshToken,
        int accessTokenExpiryMinutes,
        int refreshTokenExpiryDays,
        bool isDevelopment = false)
    {
        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment, // false in dev for http://localhost
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(accessTokenExpiryMinutes),
            Path = "/"
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpiryDays),
            Path = "/api/auth" // Only sent to auth endpoints
        };

        response.Cookies.Append(AccessTokenCookie, accessToken, accessCookieOptions);
        response.Cookies.Append(RefreshTokenCookie, refreshToken, refreshCookieOptions);
    }

    /// <summary>
    /// Clears authentication cookies
    /// </summary>
    public static void ClearAuthCookies(HttpResponse response)
    {
        var expiredOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            Path = "/"
        };

        var expiredRefreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            Path = "/api/auth"
        };

        response.Cookies.Append(AccessTokenCookie, "", expiredOptions);
        response.Cookies.Append(RefreshTokenCookie, "", expiredRefreshOptions);
    }

    /// <summary>
    /// Gets the refresh token from cookies
    /// </summary>
    public static string? GetRefreshToken(HttpRequest request)
    {
        return request.Cookies[RefreshTokenCookie];
    }
}
