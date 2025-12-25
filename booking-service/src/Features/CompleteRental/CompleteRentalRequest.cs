namespace booking_service.Features.CompleteRental;

public class CompleteRentalRequest
{
    public List<Guid> OwnerVehicleIds { get; set; } = new();
}
