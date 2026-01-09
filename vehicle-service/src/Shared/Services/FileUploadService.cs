using vehicle_service.Infra.Security;

namespace vehicle_service.Shared.Services;

public interface IFileUploadService
{
    Task<List<string>> UploadImagesAsync(List<IFormFile> images);
    Task DeleteImageAsync(string imageUrl);
}

public class FileUploadService : IFileUploadService
{
    private readonly IS3StorageService _s3StorageService;
    private readonly ILogger<FileUploadService> _logger;

    private const int MaxImagesPerUpload = 10;
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB per file

    public FileUploadService(
        IS3StorageService s3StorageService,
        ILogger<FileUploadService> logger)
    {
        _s3StorageService = s3StorageService;
        _logger = logger;
    }

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> images)
    {
        if (images == null || images.Count == 0)
            return new List<string>();

        if (images.Count > MaxImagesPerUpload)
            throw new ArgumentException($"Maximum {MaxImagesPerUpload} images allowed per upload");

        // Validate file sizes
        foreach (var image in images)
        {
            if (image.Length > MaxFileSizeBytes)
                throw new ArgumentException($"File '{image.FileName}' exceeds maximum size of 5MB");
        }

        var uploadedKeys = new List<string>();

        foreach (var image in images)
        {
            try
            {
                var objectKey = await _s3StorageService.UploadFileAsync(image, "vehicles");
                uploadedKeys.Add(objectKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image {FileName}", image.FileName);
                throw new InvalidOperationException($"Failed to upload '{image.FileName}': {ex.Message}", ex);
            }
        }

        return uploadedKeys;
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        try
        {
            await _s3StorageService.DeleteFileAsync(imageUrl);
            _logger.LogInformation("Successfully deleted image: {ImageUrl}", imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image {ImageUrl}", imageUrl);
            throw;
        }
    }
}
