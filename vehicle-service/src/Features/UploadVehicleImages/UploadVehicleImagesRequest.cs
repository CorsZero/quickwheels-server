namespace vehicle_service.Features.UploadVehicleImages;

public class UploadVehicleImagesRequest
{
    public List<IFormFile> Images { get; set; } = new();
}
