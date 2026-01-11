using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Auth.Profile.DeleteProfileImage;

[ApiController]
[Route("api/auth")]
public class DeleteProfileImageController : ControllerBase
{
    private readonly DeleteProfileImageHandler _handler;

    public DeleteProfileImageController(DeleteProfileImageHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("profile/image")]
    public async Task<IActionResult> DeleteProfileImage()
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));
        }

        var result = await _handler.Handle(userId.Value);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}
