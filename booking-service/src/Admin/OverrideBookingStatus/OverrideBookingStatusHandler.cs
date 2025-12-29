using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Admin.OverrideBookingStatus;

public class OverrideBookingStatusHandler
{
    private readonly IBookingRepository _bookingRepository;

    public OverrideBookingStatusHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(Guid bookingId, string status, string? reason)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new KeyNotFoundException("Booking not found");

        if (!Enum.TryParse<BookingStatus>(status, true, out var newStatus))
            throw new ArgumentException($"Invalid status: {status}");

        // Admin can override to any status
        booking.AdminOverrideStatus(newStatus, reason);
        await _bookingRepository.UpdateAsync(booking);

        return new
        {
            id = booking.Id,
            status = booking.Status.ToString().ToUpper(),
            rejectionReason = booking.RejectionReason,
            updatedAt = booking.UpdatedAt
        };
    }
}
