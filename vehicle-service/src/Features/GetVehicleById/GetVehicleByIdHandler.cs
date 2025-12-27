using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.GetVehicleById;

public class GetVehicleByIdHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleByIdHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<object?> Handle(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            return null;

        // Only return if active (unless viewing own vehicle - handled in controller)
        if (!vehicle.IsActive)
            return null;

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
            updatedAt = vehicle.UpdatedAt
        };
    }
}
