using Microsoft.EntityFrameworkCore;
using booking_service.Domain.Entities;
using booking_service.Domain.Enums;
using booking_service.Infra.Config;

namespace booking_service.Infra.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<List<Booking>> GetByRenterIdAsync(Guid renterId, BookingStatus? status = null, int page = 1, int limit = 10)
    {
        var query = _context.Bookings.Where(b => b.RenterId == renterId);

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByVehicleIdsAsync(List<Guid> vehicleIds, BookingStatus? status = null, int page = 1, int limit = 10)
    {
        var query = _context.Bookings.Where(b => vehicleIds.Contains(b.VehicleId));

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountByRenterIdAsync(Guid renterId, BookingStatus? status = null)
    {
        var query = _context.Bookings.Where(b => b.RenterId == renterId);

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<int> CountByVehicleIdsAsync(List<Guid> vehicleIds, BookingStatus? status = null)
    {
        var query = _context.Bookings.Where(b => vehicleIds.Contains(b.VehicleId));

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<List<Booking>> CheckAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        return await _context.Bookings
            .Where(b => b.VehicleId == vehicleId &&
                       (b.Status == BookingStatus.Approved || 
                        b.Status == BookingStatus.Active) &&
                       b.StartDate.HasValue &&
                       b.EndDate.HasValue &&
                       b.StartDate.Value <= endDate &&
                       b.EndDate.Value >= startDate)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetAllAsync(BookingStatus? status = null, int page = 1, int limit = 20)
    {
        var query = _context.Bookings.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountAllAsync(BookingStatus? status = null)
    {
        var query = _context.Bookings.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Dictionary<BookingStatus, int>> GetStatisticsAsync()
    {
        return await _context.Bookings
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<BookingAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var bookingsInRange = await _context.Bookings
            .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
            .ToListAsync();

        var bookingsByMonth = bookingsInRange
            .GroupBy(b => b.CreatedAt.ToString("yyyy-MM"))
            .Select(g => new MonthlyBooking
            {
                Month = g.Key,
                Count = g.Count()
            })
            .OrderBy(m => m.Month)
            .ToList();

        var topVehicles = bookingsInRange
            .GroupBy(b => b.VehicleId)
            .Select(g => new TopVehicle
            {
                VehicleId = g.Key,
                Bookings = g.Count()
            })
            .OrderByDescending(v => v.Bookings)
            .Take(10)
            .ToList();

        return new BookingAnalytics
        {
            BookingsByMonth = bookingsByMonth,
            TopVehicles = topVehicles
        };
    }
}
