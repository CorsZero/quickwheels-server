using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.DeleteVehicle;

public class DeleteVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task Handle(Guid vehicleId, Guid userId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        if (!vehicle.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only delete your own vehicles");

        await _vehicleRepository.DeleteAsync(vehicle);
    }
}
