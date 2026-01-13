using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile.DeleteProfileImage;

public class DeleteProfileImageHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IS3StorageService _s3StorageService;
    private readonly ILogger<DeleteProfileImageHandler> _logger;

    public DeleteProfileImageHandler(
        IUserRepository userRepository,
        IS3StorageService s3StorageService,
        ILogger<DeleteProfileImageHandler> logger)
    {
        _userRepository = userRepository;
        _s3StorageService = s3StorageService;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        // Check if user has a profile image
        if (string.IsNullOrEmpty(user.ProfileImageKey))
        {
            return ApiResponse.ErrorResult("No profile image to delete");
        }

        var imageKeyToDelete = user.ProfileImageKey;

        _logger.LogInformation("Attempting to delete profile image: {ImageKey} for user: {UserId}", 
            imageKeyToDelete, userId);

        // Delete the image from S3
        try
        {
            await _s3StorageService.DeleteFileAsync(imageKeyToDelete);
            _logger.LogInformation("Successfully deleted profile image {ImageKey} from S3", imageKeyToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete profile image {ImageKey} from S3", imageKeyToDelete);
            return ApiResponse.ErrorResult($"Failed to delete image from storage: {ex.Message}");
        }

        // Clear the profile image key in the user entity
        user.ClearProfileImage();
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(new
        {
            message = "Profile image deleted successfully",
            deletedImageKey = imageKeyToDelete
        });
    }
}
