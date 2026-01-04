using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile;

public class GetCurrentProfileHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IS3StorageService _s3StorageService;

    public GetCurrentProfileHandler(
        IUserRepository userRepository,
        IS3StorageService s3StorageService)
    {
        _userRepository = userRepository;
        _s3StorageService = s3StorageService;
    }

    public async Task<ApiResponse> Handle(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        // Generate signed URL if profile image exists
        string? profileImageUrl = null;
        if (!string.IsNullOrEmpty(user.ProfileImageKey))
        {
            profileImageUrl = _s3StorageService.GenerateSignedUrl(user.ProfileImageKey);
        }

        return ApiResponse.SuccessResult(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Phone,
            ProfileImage = profileImageUrl,
            Role = user.Role.ToString(),
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt,
            user.LastLoginAt
        });
    }
}
