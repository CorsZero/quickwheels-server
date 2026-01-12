using vehicle_service.Infra.Repositories;
using vehicle_service.Infra.Security;

namespace vehicle_service.Features.RemoveVehicle;

public class RemoveVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IS3StorageService _s3StorageService;

    public RemoveVehicleHandler(
        IVehicleRepository vehicleRepository,
        IS3StorageService s3StorageService)
    {
        _vehicleRepository = vehicleRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<object> Handle(Guid vehicleId, Guid userId, RemoveVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        if (!vehicle.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only remove your own vehicles");

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("Removal reason is required");

        // Delete all images from S3
        var imageKeys = vehicle.GetImagesList();
        if (imageKeys != null && imageKeys.Any())
        {
            foreach (var imageKey in imageKeys)
            {
                try
                {
                    await _s3StorageService.DeleteFileAsync(imageKey);
                }
                catch (Exception)
                {
                    // Continue deleting other images even if one fails
                }
            }
        }

        vehicle.Remove();
        await _vehicleRepository.UpdateAsync(vehicle);

        return new
        {
            message = "Vehicle removed successfully",
            vehicleId = vehicle.Id,
            reason = request.Reason
        };
    }
}
