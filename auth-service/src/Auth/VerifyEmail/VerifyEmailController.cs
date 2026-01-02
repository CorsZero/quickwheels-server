using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Infra.Repositories;

namespace sevaLK_service_auth.Auth.VerifyEmail;

[ApiController]
[Route("api/auth")]
public class VerifyEmailController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public VerifyEmailController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            return BadRequest("Invalid verification link");
        }

        if (user.IsActive)
        {
            return Ok("Email already verified");
        }

        user.Activate();
        await _userRepository.UpdateAsync(user);

        return Ok("Email verified successfully! You can now log in.");
    }
}
