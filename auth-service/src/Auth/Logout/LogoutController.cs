using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Logout;

[ApiController]
[Route("api/auth")]
public class LogoutController : ControllerBase
{
    private readonly LogoutHandler _handler;

    public LogoutController(LogoutHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = HttpContext.GetUserId();
        var result = await _handler.Handle(userId, Response);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
