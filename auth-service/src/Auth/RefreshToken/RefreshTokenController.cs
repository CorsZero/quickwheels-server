using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _handler.Handle(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
