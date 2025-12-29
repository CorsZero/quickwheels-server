using booking_service.Domain.Enums;
using booking_service.Infra.Repositories;

namespace booking_service.Admin.GetAllBookingsAdmin;

public class GetAllBookingsAdminHandler
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsAdminHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(string? status, int page, int limit)
    {
        BookingStatus? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
        {
            if (Enum.TryParse<BookingStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }
        }

        var bookings = await _bookingRepository.GetAllAsync(statusEnum, page, limit);
        var total = await _bookingRepository.CountAllAsync(statusEnum);
        var statistics = await _bookingRepository.GetStatisticsAsync();

        var bookingResponses = bookings.Select(b => new
        {
            id = b.Id,
            renterId = b.RenterId,
            vehicleId = b.VehicleId,
            startDate = b.StartDate,
            endDate = b.EndDate,
            days = b.Days,
            status = b.Status.ToString().ToUpper(),
            notes = b.Notes,
            rejectionReason = b.RejectionReason,
            createdAt = b.CreatedAt,
            updatedAt = b.UpdatedAt
        }).ToList();

        return new
        {
            bookings = bookingResponses,
            pagination = new
            {
                page,
                limit,
                total,
                totalPages = (int)Math.Ceiling((double)total / limit)
            },
            statistics = new
            {
                total = statistics.Values.Sum(),
                pending = statistics.GetValueOrDefault(BookingStatus.Pending, 0),
                approved = statistics.GetValueOrDefault(BookingStatus.Approved, 0),
                active = statistics.GetValueOrDefault(BookingStatus.Active, 0),
                completed = statistics.GetValueOrDefault(BookingStatus.Completed, 0),
                rejected = statistics.GetValueOrDefault(BookingStatus.Rejected, 0),
                cancelled = statistics.GetValueOrDefault(BookingStatus.Cancelled, 0)
            }
        };
    }
}
