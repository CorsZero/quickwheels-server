using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile;

public class UpdateProfileHandler
{
    private readonly IUserRepository _userRepository;

    public UpdateProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        if (string.IsNullOrEmpty(request.FullName) && string.IsNullOrEmpty(request.Phone))
        {
            return ApiResponse.ErrorResult("No changes provided");
        }

        user.UpdateProfile(request.FullName, request.Phone);
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Phone,
            Role = user.Role.ToString(),
            user.UpdatedAt
        }, "Profile updated successfully");
    }
}
