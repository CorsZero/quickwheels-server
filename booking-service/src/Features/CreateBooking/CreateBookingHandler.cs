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
        // Validate dates
        if (request.StartDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Start date cannot be in the past");

        if (request.EndDate.Date < request.StartDate.Date)
            throw new ArgumentException("End date must be after start date");

        // Check availability
        var conflictingBookings = await _bookingRepository.CheckAvailabilityAsync(
            request.VehicleId, 
            request.StartDate, 
            request.EndDate
        );

        if (conflictingBookings.Any())
            throw new InvalidOperationException("Vehicle is not available for the selected dates");

        // Create booking
        var booking = new Booking(
            renterId,
            request.VehicleId,
            request.StartDate,
            request.EndDate,
            request.Notes
        );

        await _bookingRepository.CreateAsync(booking);

        return new
        {
            id = booking.Id,
            renterId = booking.RenterId,
            vehicleId = booking.VehicleId,
            startDate = booking.StartDate,
            endDate = booking.EndDate,
            days = booking.Days,
            status = booking.Status.ToString().ToUpper(),
            notes = booking.Notes,
            createdAt = booking.CreatedAt,
            message = "Booking request created successfully"
        };
    }
}
