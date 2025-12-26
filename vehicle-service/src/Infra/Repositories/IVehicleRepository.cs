using vehicle_service.Domain.Entities;
using vehicle_service.Domain.Enums;

namespace vehicle_service.Infra.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<List<Vehicle>> GetAllAsync(
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
        int limit = 12);
    Task<int> CountAllAsync(
        string? location = null,
        string? district = null,
        VehicleCategory? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Transmission? transmission = null,
        FuelType? fuelType = null,
        int? seats = null,
        string? search = null);
    Task<List<Vehicle>> GetByOwnerIdAsync(Guid ownerId, int page = 1, int limit = 10);
    Task<int> CountByOwnerIdAsync(Guid ownerId);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
    
    // Admin methods
    Task<List<Vehicle>> GetAllAdminAsync(VehicleStatus? status = null, bool? isActive = null, int page = 1, int limit = 20);
    Task<int> CountAllAdminAsync(VehicleStatus? status = null, bool? isActive = null);
    Task<List<Vehicle>> GetByIdsAsync(List<Guid> ids);
}
