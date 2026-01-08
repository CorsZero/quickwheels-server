using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Features.GetAllIncomingRequests;

public class GetAllIncomingRequestsHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllIncomingRequestsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(GetAllIncomingRequestsRequest request)
    {
        BookingStatus? bookingStatus = null;
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (!Enum.TryParse<BookingStatus>(request.Status, true, out var parsedStatus))
            {
                throw new ArgumentException($"Invalid status value: {request.Status}. Valid values are: Pending, Approved, Rejected, Active, Completed, Cancelled");
            }
            bookingStatus = parsedStatus;
        }

        var bookings = await _bookingRepository.GetAllAsync(bookingStatus, request.Page, request.Limit);
        var total = await _bookingRepository.CountAllAsync(bookingStatus);

        return new
        {
            bookings = bookings.Select(b => new
            {
                id = b.Id,
                renterId = b.RenterId,
                vehicleId = b.VehicleId,
                startDate = b.StartDate,
                endDate = b.EndDate,
                days = b.Days,
                status = b.Status.ToString().ToUpper(),
                notes = b.Notes,
                createdAt = b.CreatedAt
            }),
            pagination = new
            {
                page = request.Page,
                limit = request.Limit,
                total,
                totalPages = (int)Math.Ceiling(total / (double)request.Limit)
            }
        };
    }
}
