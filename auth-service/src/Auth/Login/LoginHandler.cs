using sevaLK_service_auth.Infra.Repositories;
using sevaLK_service_auth.Infra.Security;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Login;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<ApiResponse> Handle(LoginRequest request, HttpResponse httpResponse)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse.ErrorResult("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return ApiResponse.ErrorResult("Account is deactivated. Please contact support.");
        }

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return ApiResponse.ErrorResult("Invalid email or password");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var accessExpiryMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
        
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        // Set tokens as HttpOnly cookies
        Cookie.SetAuthCookies(
            httpResponse,
            accessToken,
            refreshToken,
            accessExpiryMinutes,
            refreshTokenExpiryDays,
            _environment.IsDevelopment()
        );

        return ApiResponse.SuccessResult(new
        {
            ExpiresIn = accessExpiryMinutes * 60,
            User = new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.Phone,
                Role = user.Role.ToString()
            }
        }, "Login successful");
    }
}
