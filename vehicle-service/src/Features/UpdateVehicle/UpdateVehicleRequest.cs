namespace vehicle_service.Features.UpdateVehicle;

public class UpdateVehicleRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Category { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public int? Seats { get; set; }
    public decimal? PricePerDay { get; set; }
    public string? Location { get; set; }
    public string? District { get; set; }
    public string? Description { get; set; }
    public List<string>? Features { get; set; }
    public List<string>? Images { get; set; }
}
