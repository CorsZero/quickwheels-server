using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile;

[ApiController]
[Route("api/auth")]
public class ProfileController : ControllerBase
{
    private readonly GetCurrentProfileHandler _getHandler;
    private readonly UpdateProfileHandler _updateHandler;

    public ProfileController(GetCurrentProfileHandler getHandler, UpdateProfileHandler updateHandler)
    {
        _getHandler = getHandler;
        _updateHandler = updateHandler;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));
        }

        var result = await _getHandler.Handle(userId.Value);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));
        }

        var result = await _updateHandler.Handle(userId.Value, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
