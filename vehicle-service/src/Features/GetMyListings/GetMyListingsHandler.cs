using vehicle_service.Infra.Repositories;
using vehicle_service.Infra.Security;

namespace vehicle_service.Features.GetMyListings;

public class GetMyListingsHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IS3StorageService _s3StorageService;

    public GetMyListingsHandler(
        IVehicleRepository vehicleRepository,
        IS3StorageService s3StorageService)
    {
        _vehicleRepository = vehicleRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<object> Handle(Guid ownerId, int page, int limit)
    {
        var vehicles = await _vehicleRepository.GetByOwnerIdAsync(ownerId, page, limit);
        var total = await _vehicleRepository.CountByOwnerIdAsync(ownerId);

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
            images = _s3StorageService.GenerateSignedUrls(v.GetImagesList() ?? new List<string>()),
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
