using Microsoft.EntityFrameworkCore;
using vehicle_service.Domain.Entities;
using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Config;

namespace vehicle_service.Infra.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly VehicleDbContext _context;

    public VehicleRepository(VehicleDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<List<Vehicle>> GetAllAsync(
        string? location = null,
        string? district = null,
        VehicleCategory? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Transmission? transmission = null,
        FuelType? fuelType = null,
        int? seats = null,
        string? search = null,
        int page = 1,
        int limit = 12)
    {
        var query = BuildPublicQuery(location, district, category, minPrice, maxPrice, transmission, fuelType, seats, search);

        return await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountAllAsync(
        string? location = null,
        string? district = null,
        VehicleCategory? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Transmission? transmission = null,
        FuelType? fuelType = null,
        int? seats = null,
        string? search = null)
    {
        var query = BuildPublicQuery(location, district, category, minPrice, maxPrice, transmission, fuelType, seats, search);
        return await query.CountAsync();
    }

    private IQueryable<Vehicle> BuildPublicQuery(
        string? location,
        string? district,
        VehicleCategory? category,
        decimal? minPrice,
        decimal? maxPrice,
        Transmission? transmission,
        FuelType? fuelType,
        int? seats,
        string? search)
    {
        // Only show active and available vehicles publicly
        var query = _context.Vehicles
            .Where(v => v.IsActive && v.Status == VehicleStatus.Available);

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(v => v.Location.ToLower().Contains(location.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(district))
        {
            query = query.Where(v => v.District.ToLower().Contains(district.ToLower()));
        }

        if (category.HasValue)
        {
            query = query.Where(v => v.Category == category.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(v => v.PricePerDay >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(v => v.PricePerDay <= maxPrice.Value);
        }

        if (transmission.HasValue)
        {
            query = query.Where(v => v.Transmission == transmission.Value);
        }

        if (fuelType.HasValue)
        {
            query = query.Where(v => v.FuelType == fuelType.Value);
        }

        if (seats.HasValue)
        {
            query = query.Where(v => v.Seats >= seats.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(v => 
                v.Make.ToLower().Contains(searchLower) ||
                v.Model.ToLower().Contains(searchLower) ||
                (v.Description != null && v.Description.ToLower().Contains(searchLower)));
        }

        return query;
    }

    public async Task<List<Vehicle>> GetByOwnerIdAsync(Guid ownerId, int page = 1, int limit = 10)
    {
        return await _context.Vehicles
            .Where(v => v.OwnerId == ownerId)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Vehicles
            .Where(v => v.OwnerId == ownerId)
            .CountAsync();
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
    }

    // Admin methods
    public async Task<List<Vehicle>> GetAllAdminAsync(VehicleStatus? status = null, bool? isActive = null, int page = 1, int limit = 20)
    {
        var query = _context.Vehicles.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(v => v.IsActive == isActive.Value);
        }

        return await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountAllAdminAsync(VehicleStatus? status = null, bool? isActive = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(v => v.IsActive == isActive.Value);
        }

        return await query.CountAsync();
    }

    public async Task<List<Vehicle>> GetByIdsAsync(List<Guid> ids)
    {
        return await _context.Vehicles
            .Where(v => ids.Contains(v.Id))
            .ToListAsync();
    }
}
