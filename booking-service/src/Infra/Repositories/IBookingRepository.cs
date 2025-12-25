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
}
