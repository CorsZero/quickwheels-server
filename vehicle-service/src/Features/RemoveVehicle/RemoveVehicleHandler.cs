using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.RemoveVehicle;

public class RemoveVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public RemoveVehicleHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
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
