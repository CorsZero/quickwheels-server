namespace booking_service.Features.GetMyRequests;

public class GetMyRequestsRequest
{
    public List<Guid> VehicleIds { get; set; } = new();
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}
