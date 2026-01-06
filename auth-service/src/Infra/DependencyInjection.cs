using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using sevaLK_service_auth.Domain.Objects;
using sevaLK_service_auth.Infra.Config;
using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Services;

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
        
        // AWS S3 Client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var region = Environment.GetEnvironmentVariable("AWS_REGION") 
                ?? configuration["AwsS3:Region"] 
                ?? "us-east-1";
            
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
            };

            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                return new AmazonS3Client(accessKey, secretKey, config);
            }
            
            return new AmazonS3Client(config);
        });

        // S3 Storage Service
        services.AddScoped<IS3StorageService, S3StorageService>();

        // File Upload Service
        services.AddScoped<IFileUploadService, FileUploadService>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        // Email Service
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
