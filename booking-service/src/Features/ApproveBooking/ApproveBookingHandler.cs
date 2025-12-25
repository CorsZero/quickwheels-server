using booking_service.Infra.Repositories;

namespace booking_service.Features.ApproveBooking;

public class ApproveBookingHandler
{
    private readonly IBookingRepository _bookingRepository;

    public ApproveBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid ownerId, List<Guid> ownerVehicleIds)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        // Verify owner owns the vehicle
        if (!ownerVehicleIds.Contains(booking.VehicleId))
            throw new UnauthorizedAccessException("You do not have permission to approve this booking");

        booking.Approve();
        await _bookingRepository.UpdateAsync(booking);

        return new
        {
            message = "Booking approved",
            booking = new
            {
                id = booking.Id,
                status = booking.Status.ToString().ToUpper()
            }
        };
    }
}
