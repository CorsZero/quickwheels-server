using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.UpdateVehicleStatus;

public class UpdateVehicleStatusHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleStatusHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<object> Handle(Guid vehicleId, UpdateVehicleStatusRequest request, Guid userId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        if (!vehicle.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only update status of your own vehicles");

        if (!Enum.TryParse<VehicleStatus>(request.Status, true, out var status))
            throw new ArgumentException("Invalid status. Valid values: AVAILABLE, MAINTENANCE");

        // Owner can only set AVAILABLE or MAINTENANCE
        if (status != VehicleStatus.Available && status != VehicleStatus.Maintenance)
            throw new ArgumentException("You can only set status to AVAILABLE or MAINTENANCE");

        vehicle.UpdateStatus(status);
        await _vehicleRepository.UpdateAsync(vehicle);

        return new
        {
            message = "Vehicle status updated",
            status = vehicle.Status.ToString().ToUpper()
        };
    }
}
