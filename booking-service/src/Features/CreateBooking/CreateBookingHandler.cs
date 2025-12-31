using booking_service.Domain.Entities;
using booking_service.Infra.Repositories;

namespace booking_service.Features.CreateBooking;

public class CreateBookingHandler
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<object> Handle(CreateBookingRequest request, Guid renterId)
    {
        // Create booking without proposed dates
        var booking = new Booking(
            renterId,
            request.VehicleId,
            request.Notes
        );

        await _bookingRepository.CreateAsync(booking);

        return new
        {
            id = booking.Id,
            renterId = booking.RenterId,
            vehicleId = booking.VehicleId,
            status = booking.Status.ToString().ToUpper(),
            notes = booking.Notes,
            createdAt = booking.CreatedAt,
            message = "Booking request created successfully. Dates will be set when rental starts."
        };
    }
}
