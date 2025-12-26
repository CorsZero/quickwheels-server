using Microsoft.EntityFrameworkCore;
using vehicle_service.Infra.Config;
using vehicle_service.Infra.Repositories;
using vehicle_service.Infra.Security;
using vehicle_service.Features.GetAllVehicles;
using vehicle_service.Features.GetVehicleById;
using vehicle_service.Features.CreateVehicle;
using vehicle_service.Features.GetMyListings;
using vehicle_service.Features.UpdateVehicle;
using vehicle_service.Features.UpdateVehicleStatus;
using vehicle_service.Features.DeleteVehicle;
using vehicle_service.Features.GetAllVehiclesAdmin;
using vehicle_service.Features.RemoveVehicle;
using vehicle_service.Features.ActivateVehicle;

namespace vehicle_service.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<VehicleDbContext>(options =>
            options.UseNpgsql(
                configuration["DefaultConnection"],
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("vehicle-service")
            ));

        // Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        // Security - JWT validation only
        services.AddScoped<IJwtService, JwtService>();

        // Handlers - Public
        services.AddScoped<GetAllVehiclesHandler>();
        services.AddScoped<GetVehicleByIdHandler>();

        // Handlers - User
        services.AddScoped<CreateVehicleHandler>();
        services.AddScoped<GetMyListingsHandler>();
        services.AddScoped<UpdateVehicleHandler>();
        services.AddScoped<UpdateVehicleStatusHandler>();
        services.AddScoped<DeleteVehicleHandler>();

        // Handlers - Admin
        services.AddScoped<GetAllVehiclesAdminHandler>();
        services.AddScoped<RemoveVehicleHandler>();
        services.AddScoped<ActivateVehicleHandler>();

        return services;
    }
}
