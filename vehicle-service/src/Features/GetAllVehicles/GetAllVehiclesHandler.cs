using vehicle_service.Domain.Enums;
using vehicle_service.Infra.Repositories;
using vehicle_service.Infra.Security;

namespace vehicle_service.Features.GetAllVehicles;

public class GetAllVehiclesHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IS3StorageService _s3StorageService;

    public GetAllVehiclesHandler(
        IVehicleRepository vehicleRepository,
        IS3StorageService s3StorageService)
    {
        _vehicleRepository = vehicleRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<object> Handle(
        string? location,
        string? district,
        string? category,
        decimal? minPrice,
        decimal? maxPrice,
        string? transmission,
        string? fuelType,
        int? seats,
        string? search,
        int page,
        int limit)
    {
        // Parse enums
        VehicleCategory? categoryEnum = null;
        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<VehicleCategory>(category, true, out var parsedCategory))
        {
            categoryEnum = parsedCategory;
        }

        Transmission? transmissionEnum = null;
        if (!string.IsNullOrWhiteSpace(transmission) && Enum.TryParse<Transmission>(transmission, true, out var parsedTransmission))
        {
            transmissionEnum = parsedTransmission;
        }

        FuelType? fuelTypeEnum = null;
        if (!string.IsNullOrWhiteSpace(fuelType) && Enum.TryParse<FuelType>(fuelType, true, out var parsedFuelType))
        {
            fuelTypeEnum = parsedFuelType;
        }

        var vehicles = await _vehicleRepository.GetAllAsync(
            location, district, categoryEnum, minPrice, maxPrice,
            transmissionEnum, fuelTypeEnum, seats, search, page, limit);

        var total = await _vehicleRepository.CountAllAsync(
            location, district, categoryEnum, minPrice, maxPrice,
            transmissionEnum, fuelTypeEnum, seats, search);

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
            latitude = v.Latitude,
            longitude = v.Longitude,
            description = v.Description,
            features = v.GetFeaturesList(),
            images = _s3StorageService.GenerateSignedUrls(v.GetImagesList() ?? new List<string>()),
            status = v.Status.ToString().ToUpper(),
            createdAt = v.CreatedAt
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
