using booking_service.Domain.Entities;
using booking_service.Infra.Repositories;

namespace booking_service.Features.CompleteRental;

public class CompleteRentalHandler
{
    private readonly IBookingRepository _repository;

    public CompleteRentalHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<object?> Handle(Guid bookingId, Guid userId, List<Guid> ownerVehicleIds)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        
        if (booking == null)
            return null;

        // Verify user is the owner of the vehicle
        if (!ownerVehicleIds.Contains(booking.VehicleId))
            throw new UnauthorizedAccessException("You are not authorized to complete this rental");

        booking.CompleteRental();
        await _repository.UpdateAsync(booking);

        return new
        {
            message = "Rental completed successfully",
            bookingId = booking.Id,
            status = booking.Status.ToString()
        };
    }
}
