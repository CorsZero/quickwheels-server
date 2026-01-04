using sevaLK_service_auth.Infra.Security;

namespace sevaLK_service_auth.Shared.Services;

public interface IFileUploadService
{
    Task<string?> UploadProfileImageAsync(IFormFile? image);
}

public class FileUploadService : IFileUploadService
{
    private readonly IS3StorageService _s3StorageService;
    private readonly ILogger<FileUploadService> _logger;

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public FileUploadService(
        IS3StorageService s3StorageService,
        ILogger<FileUploadService> logger)
    {
        _s3StorageService = s3StorageService;
        _logger = logger;
    }

    public async Task<string?> UploadProfileImageAsync(IFormFile? image)
    {
        if (image == null || image.Length == 0)
            return null;

        if (image.Length > MaxFileSizeBytes)
            throw new ArgumentException($"File '{image.FileName}' exceeds maximum size of 5MB");

        try
        {
            var objectKey = await _s3StorageService.UploadFileAsync(image, "profiles");
            return objectKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload profile image {FileName}", image.FileName);
            throw new InvalidOperationException($"Failed to upload profile image: {ex.Message}", ex);
        }
    }
}
