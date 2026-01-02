using sevaLK_service_auth.Domain.Entities;
using sevaLK_service_auth.Domain.Enums;
using sevaLK_service_auth.Domain.Objects;
using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Register;

public class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<ApiResponse> Handle(RegisterRequest request)
    {
        // Validate passwords match
        if (request.Password != request.ConfirmPassword)
        {
            return ApiResponse.ErrorResult("Passwords do not match");
        }

        // Validate password strength
        if (request.Password.Length < 6)
        {
            return ApiResponse.ErrorResult("Password must be at least 6 characters");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return ApiResponse.ErrorResult($"User with email '{request.Email}' already exists");
        }

        // Parse role (only User allowed for registration, Admin must be created by other means)
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role) || role == UserRole.Admin)
        {
            return ApiResponse.ErrorResult("Invalid role. Only 'User' role is allowed for registration");
        }

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Create user
        var user = new User(request.Email, request.FullName, request.Phone, passwordHash, role);
        await _userRepository.AddAsync(user);

        // Send verification email
        try
        {
            await _emailService.SendVerificationEmailAsync(user.Email, user.Id, user.FullName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send verification email: {ex.Message}");
        }

        return ApiResponse.SuccessResult(new
        {
            user.Id,
            user.Email,
            user.FullName,
            Message = "Registration successful! Please check your email to verify your account."
        }, "User registered successfully. Please verify your email.");
    }
}
