using booking_service.Domain.Enums;

namespace booking_service.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid RenterId { get; private set; }
    public Guid VehicleId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Days { get; private set; }
    public BookingStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private Booking() { }

    public Booking(Guid renterId, Guid vehicleId, DateTime startDate, DateTime endDate, string? notes = null)
    {
        Id = Guid.NewGuid();
        RenterId = renterId;
        VehicleId = vehicleId;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Days = (endDate.Date - startDate.Date).Days + 1;
        Status = BookingStatus.Pending;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be approved");

        Status = BookingStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be rejected");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required");

        Status = BookingStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartRental()
    {
        if (Status != BookingStatus.Approved)
            throw new InvalidOperationException("Only approved bookings can be started");

        if (DateTime.UtcNow.Date < StartDate)
            throw new InvalidOperationException("Cannot start rental before start date");

        Status = BookingStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteRental()
    {
        if (Status != BookingStatus.Active)
            throw new InvalidOperationException("Only active rentals can be completed");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be cancelled");

        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsRenter(Guid userId) => RenterId == userId;
}
