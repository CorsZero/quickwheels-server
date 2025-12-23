using Microsoft.AspNetCore.Mvc;

namespace sevaLK_service_auth.Auth.Login;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly LoginHandler _handler;

    public LoginController(LoginHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _handler.Handle(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
