using System.Security.Claims;
using vehicle_service.Infra.Security;

namespace vehicle_service.Shared.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        var token = ExtractToken(context.Request);

        if (!string.IsNullOrEmpty(token))
        {
            AttachUserToContext(context, jwtService, token);
        }

        await _next(context);
    }

    private static string? ExtractToken(HttpRequest request)
    {
        // Try Authorization header first (Bearer token)
        var authHeader = request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader["Bearer ".Length..].Trim();
        }

        // Try query string (for WebSocket connections)
        if (request.Query.TryGetValue("access_token", out var queryToken))
        {
            return queryToken.FirstOrDefault();
        }

        // Try cookie
        if (request.Cookies.TryGetValue("access_token", out var cookieToken))
        {
            return cookieToken;
        }

        return null;
    }

    private static void AttachUserToContext(HttpContext context, IJwtService jwtService, string token)
    {
        var (isValid, userId, role) = jwtService.ValidateToken(token);

        if (isValid)
        {
            // Attach user info to context for use in controllers
            context.Items["UserId"] = userId;
            context.Items["UserRole"] = role;

            // Create claims identity for authorization
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            context.User = new ClaimsPrincipal(identity);
        }
    }
}

public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}

/// <summary>
/// Extension methods to easily get user info from HttpContext
/// </summary>
public static class HttpContextExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        return context.Items["UserId"] as Guid?;
    }

    public static string? GetUserRole(this HttpContext context)
    {
        return context.Items["UserRole"] as string;
    }

    public static bool IsAuthenticated(this HttpContext context)
    {
        return context.Items["UserId"] != null;
    }

    public static bool IsAdmin(this HttpContext context)
    {
        var role = context.GetUserRole();
        return role?.ToUpper() == "ADMIN";
    }
}
