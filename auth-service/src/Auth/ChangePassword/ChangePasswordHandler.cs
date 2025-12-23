using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.ChangePassword;

public class ChangePasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse> Handle(Guid userId, ChangePasswordRequest request)
    {
        // Validate passwords match
        if (request.NewPassword != request.ConfirmPassword)
        {
            return ApiResponse.ErrorResult("New passwords do not match");
        }

        // Validate password strength
        if (request.NewPassword.Length < 6)
        {
            return ApiResponse.ErrorResult("Password must be at least 6 characters");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResult("User not found");
        }

        // Verify current password
        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return ApiResponse.ErrorResult("Current password is incorrect");
        }

        // Update password
        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.UpdatePassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(message: "Password changed successfully");
    }
}
