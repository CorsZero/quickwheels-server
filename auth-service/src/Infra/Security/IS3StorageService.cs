using Microsoft.AspNetCore.Http;

namespace sevaLK_service_auth.Infra.Security;

public interface IS3StorageService
{
    Task<string> UploadFileAsync(IFormFile file, string prefix = "profiles");
    string GenerateSignedUrl(string objectKey, int? expiryMinutes = null);
    Task DeleteFileAsync(string objectKey);
}
