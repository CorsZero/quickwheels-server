using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Repositories;

namespace vehicle_service.Features.GetAllVehiclesAdmin;

public class GetAllVehiclesAdminHandler
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetAllVehiclesAdminHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<object> Handle(string? status, bool? isActive, int page, int limit)
    {
        VehicleStatus? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
        {
            if (Enum.TryParse<VehicleStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }
        }

        var vehicles = await _vehicleRepository.GetAllAdminAsync(statusEnum, isActive, page, limit);
        var total = await _vehicleRepository.CountAllAdminAsync(statusEnum, isActive);

        var vehicleResponses = vehicles.Select(v => new
        {
            id = v.Id,
            ownerId = v.OwnerId,
            make = v.Make,
            model = v.Model,
            year = v.Year,
            category = v.Category.ToString().ToUpper(),
            transmission = v.Transmission.ToString().ToUpper(),
            fuelType = v.FuelType.ToString().ToUpper(),
            seats = v.Seats,
            pricePerDay = v.PricePerDay,
            location = v.Location,
            district = v.District,
            description = v.Description,
            features = v.GetFeaturesList(),
            images = v.GetImagesList(),
            status = v.Status.ToString().ToUpper(),
            isActive = v.IsActive,
            createdAt = v.CreatedAt,
            updatedAt = v.UpdatedAt
        }).ToList();

        return new
        {
            vehicles = vehicleResponses,
            pagination = new
            {
                page,
                limit,
                total,
                totalPages = (int)Math.Ceiling((double)total / limit)
            }
        };
    }
}
