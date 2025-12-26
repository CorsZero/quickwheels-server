using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.ActivateVehicle;

public class ActivateVehicleHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public ActivateVehicleHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<object> Handle(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        vehicle.Activate();
        await _vehicleRepository.UpdateAsync(vehicle);

        return new
        {
            message = "Vehicle activated successfully",
            vehicleId = vehicle.Id,
            isActive = vehicle.IsActive,
            status = vehicle.Status.ToString().ToUpper()
        };
    }
}
