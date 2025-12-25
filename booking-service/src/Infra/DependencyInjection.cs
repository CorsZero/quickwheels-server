using booking_service.Infra.Security;

namespace booking_service.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Security - JWT validation only
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
