using booking_service.Infra.Repositories;

namespace booking_service.Features.GetBookingDetails;

public class GetBookingDetailsHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingDetailsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        // User can only view their own bookings as renter
        // Owner verification happens on client side after fetching vehicle
        if (!booking.IsRenter(userId))
            throw new UnauthorizedAccessException("You do not have permission to view this booking");

        return new
        {
            id = booking.Id,
            renterId = booking.RenterId,
            vehicleId = booking.VehicleId,
            startDate = booking.StartDate,
            endDate = booking.EndDate,
            days = booking.Days,
            status = booking.Status.ToString().ToUpper(),
            notes = booking.Notes,
            rejectionReason = booking.RejectionReason,
            createdAt = booking.CreatedAt,
            updatedAt = booking.UpdatedAt
        };
    }
}
