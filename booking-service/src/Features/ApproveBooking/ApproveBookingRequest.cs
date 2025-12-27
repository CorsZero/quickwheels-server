namespace booking_service.Features.ApproveBooking;

public class ApproveBookingRequest
{
    public List<Guid> OwnerVehicleIds { get; set; } = new();
}
