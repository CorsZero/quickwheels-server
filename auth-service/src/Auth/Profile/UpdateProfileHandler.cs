using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Shared.Middlewares;
using sevaLK_service_auth.Shared.Services;

namespace sevaLK_service_auth.Auth.Profile;

public class UpdateProfileHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IFileUploadService _fileUploadService;

    public UpdateProfileHandler(
        IUserRepository userRepository,
        IFileUploadService fileUploadService)
    {
        _userRepository = userRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<ApiResponse> Handle(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        if (string.IsNullOrEmpty(request.FullName) && 
            string.IsNullOrEmpty(request.Phone) && 
            request.ProfileImage == null)
        {
            return ApiResponse.ErrorResult("No changes provided");
        }

        // Upload profile image to S3 if provided
        string? profileImageKey = null;
        if (request.ProfileImage != null)
        {
            profileImageKey = await _fileUploadService.UploadProfileImageAsync(request.ProfileImage);
        }

        user.UpdateProfile(request.FullName, request.Phone, profileImageKey);
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Phone,
            ProfileImageKey = user.ProfileImageKey,
            Role = user.Role.ToString(),
            user.UpdatedAt
        }, "Profile updated successfully");
    }
}
