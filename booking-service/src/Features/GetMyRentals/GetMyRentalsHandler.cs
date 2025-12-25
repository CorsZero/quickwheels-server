using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Features.GetMyRentals;

public class GetMyRentalsHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetMyRentalsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(Guid renterId, string? status, int page, int limit)
    {
        BookingStatus? bookingStatus = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<BookingStatus>(status, true, out var parsedStatus))
            {
                bookingStatus = parsedStatus;
            }
        }

        var bookings = await _bookingRepository.GetByRenterIdAsync(renterId, bookingStatus, page, limit);
        var total = await _bookingRepository.CountByRenterIdAsync(renterId, bookingStatus);

        return new
        {
            bookings = bookings.Select(b => new
            {
                id = b.Id,
                vehicleId = b.VehicleId,
                startDate = b.StartDate,
                endDate = b.EndDate,
                days = b.Days,
                status = b.Status.ToString().ToUpper(),
                notes = b.Notes,
                createdAt = b.CreatedAt
            }),
            pagination = new
            {
                page,
                limit,
                total,
                totalPages = (int)Math.Ceiling(total / (double)limit)
            }
        };
    }
}
