using Amazon.S3;
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
using vehicle_service.Features.UploadVehicleImages;

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

        // AWS S3 Configuration
        services.Configure<AwsS3Options>(options =>
        {
            options.BucketName = Environment.GetEnvironmentVariable("AWS_S3_BUCKET_NAME") 
                ?? configuration["AwsS3:BucketName"] 
                ?? throw new InvalidOperationException("AWS S3 Bucket name is not configured");
            
            options.Region = Environment.GetEnvironmentVariable("AWS_REGION") 
                ?? configuration["AwsS3:Region"] 
                ?? "us-east-1";
            
            options.DefaultSignedUrlExpiryMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("AWS_S3_SIGNED_URL_EXPIRY_MINUTES"), 
                out var expiry) 
                ? expiry 
                : configuration.GetValue<int>("AwsS3:DefaultSignedUrlExpiryMinutes", 10);
        });
        
        // AWS S3 Client - reads credentials from environment variables or IAM role
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var region = Environment.GetEnvironmentVariable("AWS_REGION") 
                ?? configuration["AwsS3:Region"] 
                ?? "us-east-1";
            
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
            };

            // Check if credentials are in environment variables
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

            // If credentials are provided, use them; otherwise use IAM role (production)
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                return new AmazonS3Client(accessKey, secretKey, config);
            }
            
            // Use default credentials (IAM role in production, AWS CLI profile locally)
            return new AmazonS3Client(config);
        });

        // S3 Storage Service
        services.AddScoped<IS3StorageService, S3StorageService>();

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
        services.AddScoped<UploadVehicleImagesHandler>();

        // Handlers - Admin
        services.AddScoped<GetAllVehiclesAdminHandler>();
        services.AddScoped<RemoveVehicleHandler>();
        services.AddScoped<ActivateVehicleHandler>();

        return services;
    }
}
