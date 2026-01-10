using Microsoft.EntityFrameworkCore;
using booking_service.Infra.Config;
using booking_service.Infra.Repositories;
using booking_service.Infra.Security;
using booking_service.Features.CreateBooking;
using booking_service.Features.GetMyRentals;
using booking_service.Features.GetMyRequests;
using booking_service.Features.GetAllIncomingRequests;
using booking_service.Features.GetBookingDetails;
using booking_service.Features.ApproveBooking;
using booking_service.Features.RejectBooking;
using booking_service.Features.StartRental;
using booking_service.Features.CompleteRental;
using booking_service.Features.CancelBooking;
using booking_service.Features.CheckAvailability;
using booking_service.Admin.GetAllBookingsAdmin;
using booking_service.Admin.OverrideBookingStatus;
using booking_service.Admin.GetBookingAnalytics;

namespace booking_service.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<BookingDbContext>(options =>
            options.UseNpgsql(
                configuration["DefaultConnection"],
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("booking-service")
            ));

        // Repositories
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Security - JWT validation only
        services.AddScoped<IJwtService, JwtService>();

        // Handlers
        services.AddScoped<CreateBookingHandler>();
        services.AddScoped<GetMyRentalsHandler>();
        services.AddScoped<GetMyRequestsHandler>();
        services.AddScoped<GetAllIncomingRequestsHandler>();
        services.AddScoped<GetBookingDetailsHandler>();
        services.AddScoped<ApproveBookingHandler>();
        services.AddScoped<RejectBookingHandler>();
        services.AddScoped<StartRentalHandler>();
        services.AddScoped<CompleteRentalHandler>();
        services.AddScoped<CancelBookingHandler>();
        services.AddScoped<CheckAvailabilityHandler>();

        // Admin Handlers
        services.AddScoped<GetAllBookingsAdminHandler>();
        services.AddScoped<OverrideBookingStatusHandler>();
        services.AddScoped<GetBookingAnalyticsHandler>();

        return services;
    }
}
