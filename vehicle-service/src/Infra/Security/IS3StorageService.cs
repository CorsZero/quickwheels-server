using Microsoft.AspNetCore.Http;

namespace vehicle_service.Infra.Security;

public interface IS3StorageService
{
    /// <summary>
    /// Uploads a file to S3 and returns the object key.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="prefix">Optional prefix for the object key (e.g., "vehicles")</param>
    /// <returns>The S3 object key (NOT a URL)</returns>
    Task<string> UploadFileAsync(IFormFile file, string prefix = "vehicles");

    /// <summary>
    /// Generates a pre-signed URL for downloading an object.
    /// </summary>
    /// <param name="objectKey">The S3 object key</param>
    /// <param name="expiryMinutes">URL expiry time in minutes (null uses default)</param>
    /// <returns>Pre-signed GET URL</returns>
    string GenerateSignedUrl(string objectKey, int? expiryMinutes = null);

    /// <summary>
    /// Generates signed URLs for multiple object keys.
    /// </summary>
    /// <param name="objectKeys">List of S3 object keys</param>
    /// <param name="expiryMinutes">URL expiry time in minutes (null uses default)</param>
    /// <returns>List of pre-signed GET URLs</returns>
    List<string> GenerateSignedUrls(IEnumerable<string> objectKeys, int? expiryMinutes = null);

    /// <summary>
    /// Deletes an object from S3.
    /// </summary>
    /// <param name="objectKey">The S3 object key to delete</param>
    Task DeleteFileAsync(string objectKey);
}
