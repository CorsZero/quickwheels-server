using System.Text.Json;
using vehicle_service.Domain.Enums;

namespace vehicle_service.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public VehicleCategory Category { get; private set; }
    public Transmission Transmission { get; private set; }
    public FuelType FuelType { get; private set; }
    public int Seats { get; private set; }
    public decimal PricePerDay { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Features { get; private set; } // JSON array stored as string
    public string? Images { get; private set; } // JSON array stored as string
    public VehicleStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private Vehicle() { }

    public Vehicle(
        Guid ownerId,
        string make,
        string model,
        int year,
        VehicleCategory category,
        Transmission transmission,
        FuelType fuelType,
        int seats,
        decimal pricePerDay,
        string location,
        string district,
        string? description = null,
        List<string>? features = null,
        List<string>? images = null)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Make = make;
        Model = model;
        Year = year;
        Category = category;
        Transmission = transmission;
        FuelType = fuelType;
        Seats = seats;
        PricePerDay = pricePerDay;
        Location = location;
        District = district;
        Description = description;
        Features = features != null ? JsonSerializer.Serialize(features) : null;
        Images = images != null ? JsonSerializer.Serialize(images) : null;
        Status = VehicleStatus.Available;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string? make = null,
        string? model = null,
        int? year = null,
        VehicleCategory? category = null,
        Transmission? transmission = null,
        FuelType? fuelType = null,
        int? seats = null,
        decimal? pricePerDay = null,
        string? location = null,
        string? district = null,
        string? description = null,
        List<string>? features = null,
        List<string>? images = null)
    {
        if (make != null) Make = make;
        if (model != null) Model = model;
        if (year.HasValue) Year = year.Value;
        if (category.HasValue) Category = category.Value;
        if (transmission.HasValue) Transmission = transmission.Value;
        if (fuelType.HasValue) FuelType = fuelType.Value;
        if (seats.HasValue) Seats = seats.Value;
        if (pricePerDay.HasValue) PricePerDay = pricePerDay.Value;
        if (location != null) Location = location;
        if (district != null) District = district;
        if (description != null) Description = description;
        if (features != null) Features = JsonSerializer.Serialize(features);
        if (images != null) Images = JsonSerializer.Serialize(images);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(VehicleStatus status)
    {
        // Owner can only set AVAILABLE or MAINTENANCE
        if (status != VehicleStatus.Available && status != VehicleStatus.Maintenance)
            throw new InvalidOperationException("Owner can only set status to AVAILABLE or MAINTENANCE");

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRented()
    {
        if (Status != VehicleStatus.Available)
            throw new InvalidOperationException("Only available vehicles can be rented");

        Status = VehicleStatus.Rented;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvailable()
    {
        Status = VehicleStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Remove()
    {
        Status = VehicleStatus.Removed;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = VehicleStatus.Available;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOwner(Guid userId) => OwnerId == userId;

    public List<string> GetFeaturesList()
    {
        if (string.IsNullOrEmpty(Features)) return new List<string>();
        return JsonSerializer.Deserialize<List<string>>(Features) ?? new List<string>();
    }

    public List<string> GetImagesList()
    {
        if (string.IsNullOrEmpty(Images)) return new List<string>();
        return JsonSerializer.Deserialize<List<string>>(Images) ?? new List<string>();
    }
}
