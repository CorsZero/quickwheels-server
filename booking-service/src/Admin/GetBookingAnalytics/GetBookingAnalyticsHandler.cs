using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Admin.GetBookingAnalytics;

public class GetBookingAnalyticsHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingAnalyticsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(DateTime? startDate, DateTime? endDate)
    {
        // Default to current year if not specified
        var start = startDate ?? new DateTime(DateTime.UtcNow.Year, 1, 1);
        var end = endDate ?? DateTime.UtcNow;

        var analytics = await _bookingRepository.GetAnalyticsAsync(start, end);
        var statistics = await _bookingRepository.GetStatisticsAsync();

        return new
        {
            totalBookings = statistics.Values.Sum(),
            bookingsByStatus = new
            {
                pending = statistics.GetValueOrDefault(BookingStatus.Pending, 0),
                approved = statistics.GetValueOrDefault(BookingStatus.Approved, 0),
                active = statistics.GetValueOrDefault(BookingStatus.Active, 0),
                completed = statistics.GetValueOrDefault(BookingStatus.Completed, 0),
                rejected = statistics.GetValueOrDefault(BookingStatus.Rejected, 0),
                cancelled = statistics.GetValueOrDefault(BookingStatus.Cancelled, 0)
            },
            bookingsByMonth = analytics.BookingsByMonth,
            topVehicles = analytics.TopVehicles
        };
    }
}
