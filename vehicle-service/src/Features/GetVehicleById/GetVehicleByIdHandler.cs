using vehicle_service.Infra.Repositories;
using vehicle_service.Infra.Security;

namespace vehicle_service.Features.GetVehicleById;

public class GetVehicleByIdHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IS3StorageService _s3StorageService;

    public GetVehicleByIdHandler(
        IVehicleRepository vehicleRepository,
        IS3StorageService s3StorageService)
    {
        _vehicleRepository = vehicleRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<object?> Handle(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            return null;

        // Only return if active (unless viewing own vehicle - handled in controller)
        if (!vehicle.IsActive)
            return null;

        // Convert stored object keys to signed URLs at response time
        var imageKeys = vehicle.GetImagesList();
        var signedImageUrls = imageKeys != null 
            ? _s3StorageService.GenerateSignedUrls(imageKeys) 
            : new List<string>();

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
            images = signedImageUrls, // Signed URLs instead of object keys
            status = vehicle.Status.ToString().ToUpper(),
            isActive = vehicle.IsActive,
            createdAt = vehicle.CreatedAt,
            updatedAt = vehicle.UpdatedAt
        };
    }
}
