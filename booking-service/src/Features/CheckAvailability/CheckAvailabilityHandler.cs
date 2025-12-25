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
        var isAvailable = await _repository.CheckAvailabilityAsync(vehicleId, startDate, endDate);

        return new
        {
            available = isAvailable,
            vehicleId,
            startDate,
            endDate
        };
    }
}
