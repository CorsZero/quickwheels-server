using System.Security.Cryptography;
using sevaLK_service_auth.Domain.Objects;
using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.ResetPassword;

public class ResetPasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public ResetPasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    /// <summary>
    /// Request a password reset token (would typically send email)
    /// </summary>
    public async Task<ApiResponse> HandleRequestReset(RequestPasswordResetRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Always return success to prevent email enumeration
        if (user == null)
        {
            return ApiResponse.SuccessResult(message: "If the email exists, a reset link has been sent");
        }

        // Generate reset token
        var token = GenerateResetToken();
        var expiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

        user.SetPasswordResetToken(token, expiry);
        await _userRepository.UpdateAsync(user);

        // Send password reset email
        try
        {
            await _emailService.SendPasswordResetEmailAsync(request.Email, token, user.FullName);
        }
        catch (Exception ex)
        {
            // Log the error but don't expose it to prevent email enumeration
            Console.WriteLine($"Failed to send reset email: {ex.Message}");
            // Still return success to prevent email enumeration
        }

        return ApiResponse.SuccessResult(message: "If the email exists, a reset link has been sent");
    }

    /// <summary>
    /// Reset password using token
    /// </summary>
    public async Task<ApiResponse> HandleReset(ResetPasswordRequest request)
    {
        // Validate passwords match
        if (request.NewPassword != request.ConfirmPassword)
        {
            return ApiResponse.ErrorResult("Passwords do not match");
        }

        // Validate password strength
        if (request.NewPassword.Length < 6)
        {
            return ApiResponse.ErrorResult("Password must be at least 6 characters");
        }

        // Find user
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse.ErrorResult("Invalid reset request");
        }

        // Validate token
        if (!user.IsPasswordResetTokenValid() || user.PasswordResetToken != request.Token)
        {
            return ApiResponse.ErrorResult("Invalid or expired reset token");
        }

        // Update password
        var passwordHash = _passwordHasher.Hash(request.NewPassword);
        user.UpdatePassword(passwordHash);
        await _userRepository.UpdateAsync(user);

        return ApiResponse.SuccessResult(message: "Password has been reset successfully");
    }

    private static string GenerateResetToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var code = Random.Shared.Next(100000, 999999);
        return code.ToString();
    }
}
