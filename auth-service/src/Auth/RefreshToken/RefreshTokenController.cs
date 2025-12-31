using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Infra.Security;

namespace sevaLK_service_auth.Auth.RefreshToken;

[ApiController]
[Route("api/auth")]
public class RefreshTokenController : ControllerBase
{
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenController(RefreshTokenHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Cookie.GetRefreshToken(Request);
        var result = await _handler.Handle(refreshToken, Response);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
