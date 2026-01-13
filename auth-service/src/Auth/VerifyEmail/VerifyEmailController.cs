using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Auth.VerifyEmail;

[ApiController]
[Route("api/auth")]
public class VerifyEmailController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public VerifyEmailController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid id)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            // Redirect to frontend with error
            return Redirect($"{frontendUrl}/email-verified?status=error&message=Invalid+verification+link");
        }

        if (user.IsActive)
        {
            // Redirect to frontend with already verified status
            return Redirect($"{frontendUrl}/email-verified?status=already-verified");
        }

        user.Activate();
        await _userRepository.UpdateAsync(user);

        // Redirect to frontend with success
        return Redirect($"{frontendUrl}/email-verified?status=success");
    }
}
