using Microsoft.EntityFrameworkCore;
using sevaLK_service_auth.Domain.Objects;
using sevaLK_service_auth.Infra.Config;
using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;

namespace sevaLK_service_auth.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(
                configuration["DefaultConnection"],
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("sevaLK-service-auth")
            ));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        // Email Service
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
