using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Logout;

public class LogoutHandler
{
    private readonly IUserRepository _userRepository;

    public LogoutHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        // Clear refresh token
        user.UpdateRefreshToken(null, null);
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(message: "Logged out successfully");
    }
}
