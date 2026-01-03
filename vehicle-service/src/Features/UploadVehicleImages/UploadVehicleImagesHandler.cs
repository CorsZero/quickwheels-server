using vehicle_service.Infra.Security;

namespace vehicle_service.Features.UploadVehicleImages;

public class UploadVehicleImagesHandler
{
    private readonly IS3StorageService _s3StorageService;
    private readonly ILogger<UploadVehicleImagesHandler> _logger;

    private const int MaxImagesPerUpload = 10;
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB per file

    public UploadVehicleImagesHandler(
        IS3StorageService s3StorageService,
        ILogger<UploadVehicleImagesHandler> logger)
    {
        _s3StorageService = s3StorageService;
        _logger = logger;
    }

    public async Task<UploadVehicleImagesResponse> Handle(List<IFormFile> images, Guid ownerId)
    {
        if (images == null || images.Count == 0)
            throw new ArgumentException("At least one image is required");

        if (images.Count > MaxImagesPerUpload)
            throw new ArgumentException($"Maximum {MaxImagesPerUpload} images allowed per upload");

        // Validate file sizes
        foreach (var image in images)
        {
            if (image.Length > MaxFileSizeBytes)
                throw new ArgumentException($"File '{image.FileName}' exceeds maximum size of 5MB");
        }

        var uploadedKeys = new List<string>();
        var errors = new List<string>();

        foreach (var image in images)
        {
            try
            {
                // Upload to S3 with owner-specific prefix for organization
                var objectKey = await _s3StorageService.UploadFileAsync(image, $"vehicles/{ownerId}");
                uploadedKeys.Add(objectKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image {FileName}", image.FileName);
                errors.Add($"Failed to upload '{image.FileName}': {ex.Message}");
            }
        }

        if (uploadedKeys.Count == 0 && errors.Count > 0)
            throw new InvalidOperationException($"All uploads failed: {string.Join(", ", errors)}");

        return new UploadVehicleImagesResponse
        {
            ObjectKeys = uploadedKeys,  // Store these in your database
            UploadedCount = uploadedKeys.Count,
            Errors = errors
        };
    }
}

public class UploadVehicleImagesResponse
{
    public List<string> ObjectKeys { get; set; } = new();
    public int UploadedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
