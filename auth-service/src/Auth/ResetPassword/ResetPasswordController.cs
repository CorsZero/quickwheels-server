using Microsoft.AspNetCore.Mvc;

namespace sevaLK_service_auth.Auth.ResetPassword;

[ApiController]
[Route("api/auth")]
public class ResetPasswordController : ControllerBase
{
    private readonly ResetPasswordHandler _handler;

    public ResetPasswordController(ResetPasswordHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetRequest request)
    {
        var result = await _handler.HandleRequestReset(request);
        return Ok(result); // Always return 200 to prevent email enumeration
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _handler.HandleReset(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
