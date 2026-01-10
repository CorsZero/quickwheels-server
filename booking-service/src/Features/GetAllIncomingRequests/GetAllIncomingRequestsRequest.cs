namespace booking_service.Features.GetAllIncomingRequests;

public class GetAllIncomingRequestsRequest
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}
