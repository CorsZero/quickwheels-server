using vehicle_service.Infra.Repositories;
using vehicle_service.Shared.Services;

namespace vehicle_service.Features.DeleteVehicleImage;

public class DeleteVehicleImageHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<DeleteVehicleImageHandler> _logger;

    public DeleteVehicleImageHandler(
        IVehicleRepository vehicleRepository,
        IFileUploadService fileUploadService,
        ILogger<DeleteVehicleImageHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    public async Task<object> Handle(Guid vehicleId, Guid userId, DeleteVehicleImageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            throw new ArgumentException("Image URL is required");

        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
            throw new KeyNotFoundException("Vehicle not found");

        if (!vehicle.IsOwner(userId))
            throw new UnauthorizedAccessException("You can only delete images from your own vehicles");

        // Get current images (stored as S3 keys)
        var currentImages = vehicle.GetImagesList() ?? new List<string>();

        // Extract the S3 key from the provided URL (remove query parameters and domain)
        var imageKeyToDelete = ExtractS3Key(request.ImageUrl);
        
        _logger.LogInformation("Attempting to delete image key: {ImageKey}", imageKeyToDelete);
        _logger.LogInformation("Current images in vehicle: {CurrentImages}", string.Join(", ", currentImages));

        // Find matching image by comparing S3 keys
        var matchingImage = currentImages.FirstOrDefault(img => 
        {
            var storedKey = ExtractS3Key(img);
            return storedKey.Equals(imageKeyToDelete, StringComparison.OrdinalIgnoreCase);
        });

        if (matchingImage == null)
            throw new ArgumentException($"Image not found in vehicle's images. Provided key: {imageKeyToDelete}");

        // Remove the matching image from the list
        var updatedImages = currentImages.Where(img => !ExtractS3Key(img).Equals(imageKeyToDelete, StringComparison.OrdinalIgnoreCase)).ToList();

        // Delete the image from S3 using the stored key
        try
        {
            await _fileUploadService.DeleteImageAsync(matchingImage);
            _logger.LogInformation("Successfully deleted image {ImageKey} from S3", matchingImage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image {ImageKey} from S3", matchingImage);
            throw new InvalidOperationException($"Failed to delete image from storage: {ex.Message}");
        }

        // Update the vehicle with the new images list
        vehicle.Update(images: updatedImages);
        await _vehicleRepository.UpdateAsync(vehicle);

        return new
        {
            message = "Image deleted successfully",
            deletedImageUrl = matchingImage,
            remainingImages = updatedImages,
            remainingImagesCount = updatedImages.Count
        };
    }

    // Helper method to extract S3 key from URL (handles both presigned URLs and plain keys)
    private string ExtractS3Key(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return string.Empty;

        // If it's already just a key (no http/https), return as is
        if (!imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return imageUrl;

        try
        {
            // Parse the URL and remove query parameters
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            
            // Remove leading slash and extract the key (everything after the bucket name in the path)
            // For URLs like: https://bucket.s3.region.amazonaws.com/vehicles/image.png
            // We want: vehicles/image.png
            var key = path.TrimStart('/');
            
            return key;
        }
        catch
        {
            // If parsing fails, return the original string
            return imageUrl;
        }
    }
}
