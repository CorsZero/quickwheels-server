using booking_service.Infra.Repositories;

namespace booking_service.Features.StartRental;

public class StartRentalHandler
{
    private readonly IBookingRepository _bookingRepository;

    public StartRentalHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid ownerId, List<Guid> ownerVehicleIds)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        if (!ownerVehicleIds.Contains(booking.VehicleId))
            throw new UnauthorizedAccessException(""You do not have permission to start this rental"");

        booking.StartRental();
        await _bookingRepository.UpdateAsync(booking);

        return new
        {
            message = ""Rental started"",
            booking = new
            {
                id = booking.Id,
                status = booking.Status.ToString().ToUpper()
            }
        };
    }
}
