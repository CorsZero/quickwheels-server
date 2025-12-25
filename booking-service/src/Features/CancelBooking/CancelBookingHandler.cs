using booking_service.Domain.Entities;
using booking_service.Infra.Repositories;

namespace booking_service.Features.CancelBooking;

public class CancelBookingHandler
{
    private readonly IBookingRepository _repository;

    public CancelBookingHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid userId)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        
        if (booking == null)
            return null;

        // Verify user is the renter
        if (!booking.IsRenter(userId))
            throw new UnauthorizedAccessException("You are not authorized to cancel this booking");

        booking.Cancel();
        await _repository.UpdateAsync(booking);

        return new
        {
            message = "Booking cancelled successfully",
            bookingId = booking.Id,
            status = booking.Status.ToString()
        };
    }
}
