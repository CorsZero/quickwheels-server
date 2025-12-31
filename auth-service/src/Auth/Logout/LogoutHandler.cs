using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Logout;

public class LogoutHandler
{
    private readonly IUserRepository _userRepository;

    public LogoutHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(Guid? userId, HttpResponse httpResponse)
    {
        // Clear cookies regardless of auth state
        Cookie.ClearAuthCookies(httpResponse);

        if (userId == null)
        {
            return ApiResponse.SuccessResult(message: "Logged out successfully");
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user != null)
        {
            // Clear refresh token in database
            user.UpdateRefreshToken(null, null);
            await _userRepository.UpdateAsync(user);
        }

        return ApiResponse.SuccessResult(message: "Logged out successfully");
    }
}
