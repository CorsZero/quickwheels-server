using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using vehicle_service.Infra.Config;

namespace vehicle_service.Infra.Security;

public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsS3Options _options;
    private readonly ILogger<S3StorageService> _logger;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    private static readonly Dictionary<string, string> ExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" }
    };

    public S3StorageService(
        IAmazonS3 s3Client,
        IOptions<AwsS3Options> options,
        ILogger<S3StorageService> logger)
    {
        _s3Client = s3Client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string prefix = "vehicles")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        // Validate content type
        var contentType = GetContentType(file);
        if (!AllowedContentTypes.Contains(contentType))
            throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", AllowedContentTypes)}");

        // Generate unique object key
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension))
            extension = ".jpg"; // Default extension

        var objectKey = $"{prefix}/{Guid.NewGuid()}{extension}";

        try
        {
            using var stream = file.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                InputStream = stream,
                ContentType = contentType,
                // Ensure object is private
                CannedACL = S3CannedACL.Private
            };

            await _s3Client.PutObjectAsync(putRequest);

            _logger.LogInformation("Successfully uploaded file to S3: {ObjectKey}", objectKey);

            return objectKey;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to S3: {ObjectKey}", objectKey);
            throw new InvalidOperationException($"Failed to upload file to S3: {ex.Message}", ex);
        }
    }

    public string GenerateSignedUrl(string objectKey, int? expiryMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("Object key cannot be empty");

        var expiry = expiryMinutes ?? _options.DefaultSignedUrlExpiryMinutes;

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddMinutes(expiry),
                Verb = HttpVerb.GET
            };

            var url = _s3Client.GetPreSignedURL(request);

            _logger.LogDebug("Generated signed URL for {ObjectKey} with {ExpiryMinutes} min expiry", objectKey, expiry);

            return url;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed URL for: {ObjectKey}", objectKey);
            throw new InvalidOperationException($"Failed to generate signed URL: {ex.Message}", ex);
        }
    }

    public List<string> GenerateSignedUrls(IEnumerable<string> objectKeys, int? expiryMinutes = null)
    {
        if (objectKeys == null)
            return new List<string>();

        return objectKeys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => GenerateSignedUrl(key, expiryMinutes))
            .ToList();
    }

    public async Task DeleteFileAsync(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("Object key cannot be empty");

        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);

            _logger.LogInformation("Successfully deleted file from S3: {ObjectKey}", objectKey);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file from S3: {ObjectKey}", objectKey);
            throw new InvalidOperationException($"Failed to delete file from S3: {ex.Message}", ex);
        }
    }

    private static string GetContentType(IFormFile file)
    {
        // First try to use the provided content type
        if (!string.IsNullOrEmpty(file.ContentType) && AllowedContentTypes.Contains(file.ContentType))
            return file.ContentType;

        // Fallback to extension-based detection
        var extension = Path.GetExtension(file.FileName);
        if (!string.IsNullOrEmpty(extension) && ExtensionToContentType.TryGetValue(extension, out var contentType))
            return contentType;

        // Default to jpeg
        return "image/jpeg";
    }
}
