namespace booking_service.Features.StartRental;

public class StartRentalRequest
{
    public List<Guid> OwnerVehicleIds { get; set; } = new();
}
