using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.ChangePassword;

[ApiController]
[Route("api/auth")]
public class ChangePasswordController : ControllerBase
{
    private readonly ChangePasswordHandler _handler;

    public ChangePasswordController(ChangePasswordHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.ErrorResult("Not authenticated"));
        }

        var result = await _handler.Handle(userId.Value, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
