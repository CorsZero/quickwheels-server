using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Admin.SuspendActivateUser;

[ApiController]
[Route("api/admin/users")]
public class SuspendActivateUserController : ControllerBase
{
    private readonly SuspendActivateUserHandler _handler;

    public SuspendActivateUserController(SuspendActivateUserHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{userId:guid}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        Guid userId,
        [FromBody] UpdateUserStatusRequest request)
    {
        var currentUserId = HttpContext.Items["UserId"] as Guid?;
        if (!currentUserId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(userId, request.IsActive);
            return Ok(ApiResponse.SuccessResult(result, "User status updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while updating user status"));
        }
    }
}

public class UpdateUserStatusRequest
{
    public bool IsActive { get; set; }
}
