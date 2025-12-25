namespace booking_service.Features.CreateBooking;

public class CreateBookingRequest
{
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}
