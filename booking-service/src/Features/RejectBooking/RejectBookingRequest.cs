namespace booking_service.Features.RejectBooking;

public class RejectBookingRequest
{
    public List<Guid> OwnerVehicleIds { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
}
