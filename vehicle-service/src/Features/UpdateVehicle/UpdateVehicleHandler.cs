using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Repositories;
using vehicle_service.Shared.Services;

namespace vehicle_service.Features.UpdateVehicle;

public class UpdateVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFileUploadService _fileUploadService;

    public UpdateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IFileUploadService fileUploadService)
    {
        _vehicleRepository = vehicleRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<object> Handle(Guid vehicleId, UpdateVehicleRequest request, Guid userId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        if (!vehicle.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only update your own vehicles");

        // Parse optional enums
        VehicleCategory? category = null;
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            if (!Enum.TryParse<VehicleCategory>(request.Category, true, out var parsedCategory))
                throw new ArgumentException("Invalid category. Valid values: CAR, VAN, SUV, BIKE");
            category = parsedCategory;
        }

        Transmission? transmission = null;
        if (!string.IsNullOrWhiteSpace(request.Transmission))
        {
            if (!Enum.TryParse<Transmission>(request.Transmission, true, out var parsedTransmission))
                throw new ArgumentException("Invalid transmission. Valid values: MANUAL, AUTOMATIC");
            transmission = parsedTransmission;
        }

        FuelType? fuelType = null;
        if (!string.IsNullOrWhiteSpace(request.FuelType))
        {
            if (!Enum.TryParse<FuelType>(request.FuelType, true, out var parsedFuelType))
                throw new ArgumentException("Invalid fuel type. Valid values: PETROL, DIESEL, ELECTRIC, HYBRID");
            fuelType = parsedFuelType;
        }

        // Validate optional fields if provided
        if (request.Year.HasValue && (request.Year < 1900 || request.Year > DateTime.UtcNow.Year + 1))
            throw new ArgumentException("Invalid year");

        if (request.Seats.HasValue && request.Seats <= 0)
            throw new ArgumentException("Seats must be greater than 0");

        if (request.PricePerDay.HasValue && request.PricePerDay <= 0)
            throw new ArgumentException("Price per day must be greater than 0");

        // Upload new images to S3 if provided
        List<string>? imageKeys = null;
        if (request.Images != null && request.Images.Count > 0)
        {
            imageKeys = await _fileUploadService.UploadImagesAsync(request.Images);
        }

        vehicle.Update(
            request.Make,
            request.Model,
            request.Year,
            category,
            transmission,
            fuelType,
            request.Seats,
            request.PricePerDay,
            request.Location,
            request.District,
            request.Description,
            request.Features,
            imageKeys
        );

        await _vehicleRepository.UpdateAsync(vehicle);

        return new
        {
            message = "Vehicle updated successfully",
            vehicle = new
            {
                id = vehicle.Id,
                ownerId = vehicle.OwnerId,
                make = vehicle.Make,
                model = vehicle.Model,
                year = vehicle.Year,
                category = vehicle.Category.ToString().ToUpper(),
                transmission = vehicle.Transmission.ToString().ToUpper(),
                fuelType = vehicle.FuelType.ToString().ToUpper(),
                seats = vehicle.Seats,
                pricePerDay = vehicle.PricePerDay,
                location = vehicle.Location,
                district = vehicle.District,
                description = vehicle.Description,
                features = vehicle.GetFeaturesList(),
                images = vehicle.GetImagesList(),
                status = vehicle.Status.ToString().ToUpper(),
                isActive = vehicle.IsActive,
                createdAt = vehicle.CreatedAt,
                updatedAt = vehicle.UpdatedAt
            }
        };
    }
}
