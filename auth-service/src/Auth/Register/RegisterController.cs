using Microsoft.AspNetCore.Mvc;

namespace sevaLK_service_auth.Auth.Register;

[ApiController]
[Route("api/auth")]
public class RegisterController : ControllerBase
{
    private readonly RegisterHandler _handler;

    public RegisterController(RegisterHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _handler.Handle(request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }
}
