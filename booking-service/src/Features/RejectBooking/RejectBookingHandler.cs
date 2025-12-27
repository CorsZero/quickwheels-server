using booking_service.Infra.Repositories;

namespace booking_service.Features.RejectBooking;

public class RejectBookingHandler
{
    private readonly IBookingRepository _bookingRepository;

    public RejectBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid ownerId, List<Guid> ownerVehicleIds, string reason)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        // Verify owner owns the vehicle
        if (!ownerVehicleIds.Contains(booking.VehicleId))
            throw new UnauthorizedAccessException("You do not have permission to reject this booking");

        booking.Reject(reason);
        await _bookingRepository.UpdateAsync(booking);

        return new
        {
            message = "Booking rejected",
            booking = new
            {
                id = booking.Id,
                status = booking.Status.ToString().ToUpper(),
                rejectionReason = booking.RejectionReason
            }
        };
    }
}
