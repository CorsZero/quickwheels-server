using vehicle_service.Domain.Entities;
using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Repositories;
using vehicle_service.Shared.Services;

namespace vehicle_service.Features.CreateVehicle;

public class CreateVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFileUploadService _fileUploadService;

    public CreateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IFileUploadService fileUploadService)
    {
        _vehicleRepository = vehicleRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<object> Handle(CreateVehicleRequest request, Guid ownerId)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Make))
            throw new ArgumentException("Make is required");

        if (string.IsNullOrWhiteSpace(request.Model))
            throw new ArgumentException("Model is required");

        if (request.Year < 1900 || request.Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year");

        if (request.Seats <= 0)
            throw new ArgumentException("Seats must be greater than 0");

        if (request.PricePerDay <= 0)
            throw new ArgumentException("Price per day must be greater than 0");

        if (string.IsNullOrWhiteSpace(request.Location))
            throw new ArgumentException("Location is required");

        if (string.IsNullOrWhiteSpace(request.District))
            throw new ArgumentException("District is required");

        // Parse enums
        if (!Enum.TryParse<VehicleCategory>(request.Category, true, out var category))
            throw new ArgumentException("Invalid category. Valid values: CAR, VAN, SUV, BIKE");

        if (!Enum.TryParse<Transmission>(request.Transmission, true, out var transmission))
            throw new ArgumentException("Invalid transmission. Valid values: MANUAL, AUTOMATIC");

        if (!Enum.TryParse<FuelType>(request.FuelType, true, out var fuelType))
            throw new ArgumentException("Invalid fuel type. Valid values: PETROL, DIESEL, ELECTRIC, HYBRID");

        // Upload images to S3 if provided
        List<string>? imageKeys = null;
        if (request.Images != null && request.Images.Count > 0)
        {
            imageKeys = await _fileUploadService.UploadImagesAsync(request.Images);
        }

        // Create vehicle
        var vehicle = new Vehicle(
            ownerId,
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

        await _vehicleRepository.CreateAsync(vehicle);

        return new
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
            message = "Vehicle listed successfully"
        };
    }
}
