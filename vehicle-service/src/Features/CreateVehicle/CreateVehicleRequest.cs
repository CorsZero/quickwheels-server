namespace vehicle_service.Features.CreateVehicle;

public class CreateVehicleRequest
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public int Seats { get; set; }
    public decimal PricePerDay { get; set; }
    public string Location { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Description { get; set; }
    public List<string>? Features { get; set; }
    public List<IFormFile>? Images { get; set; }
}
