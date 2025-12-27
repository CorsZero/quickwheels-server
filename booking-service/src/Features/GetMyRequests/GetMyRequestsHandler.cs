using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Features.GetMyRequests;

public class GetMyRequestsHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetMyRequestsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(GetMyRequestsRequest request)
    {
        if (!request.VehicleIds.Any())
        {
            return new
            {
                bookings = new List<object>(),
                pagination = new
                {
                    page = request.Page,
                    limit = request.Limit,
                    total = 0,
                    totalPages = 0
                }
            };
        }

        BookingStatus? bookingStatus = null;
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<BookingStatus>(request.Status, true, out var parsedStatus))
            {
                bookingStatus = parsedStatus;
            }
        }

        var bookings = await _bookingRepository.GetByVehicleIdsAsync(request.VehicleIds, bookingStatus, request.Page, request.Limit);
        var total = await _bookingRepository.CountByVehicleIdsAsync(request.VehicleIds, bookingStatus);

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
