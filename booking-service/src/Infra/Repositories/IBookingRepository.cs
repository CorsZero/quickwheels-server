using booking_service.Domain.Entities;
using booking_service.Domain.Enums;

namespace booking_service.Infra.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<List<Booking>> GetByRenterIdAsync(Guid renterId, BookingStatus? status = null, int page = 1, int limit = 10);
    Task<List<Booking>> GetByVehicleIdsAsync(List<Guid> vehicleIds, BookingStatus? status = null, int page = 1, int limit = 10);
    Task<int> CountByRenterIdAsync(Guid renterId, BookingStatus? status = null);
    Task<int> CountByVehicleIdsAsync(List<Guid> vehicleIds, BookingStatus? status = null);
    Task<List<Booking>> CheckAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<List<Booking>> GetAllAsync(BookingStatus? status = null, int page = 1, int limit = 20);
    Task<int> CountAllAsync(BookingStatus? status = null);
    Task<Dictionary<BookingStatus, int>> GetStatisticsAsync();
    Task<BookingAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate);
}

public class BookingAnalytics
{
    public List<MonthlyBooking> BookingsByMonth { get; set; } = new();
    public List<TopVehicle> TopVehicles { get; set; } = new();
}

public class MonthlyBooking
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopVehicle
{
    public Guid VehicleId { get; set; }
    public int Bookings { get; set; }
}
