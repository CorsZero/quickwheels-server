using booking_service.Infra.Repositories;

namespace booking_service.Features.CheckAvailability;

public class CheckAvailabilityHandler
{
    private readonly IBookingRepository _repository;

    public CheckAvailabilityHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> Handle(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        var conflictingBookings = await _repository.CheckAvailabilityAsync(vehicleId, startDate, endDate);
        
        var isAvailable = !conflictingBookings.Any();

        return new
        {
            isAvailable = isAvailable,
            message = isAvailable 
                ? "Vehicle is available for the selected dates" 
                : "Vehicle is not available for the selected dates",
            vehicleId = vehicleId,
            requestedStartDate = startDate,
            requestedEndDate = endDate,
            conflictingBookings = conflictingBookings.Select(b => new
            {
                bookingId = b.Id,
                status = b.Status.ToString().ToUpper(),
                startDate = b.StartDate,
                endDate = b.EndDate
            })
        };
    }
}
