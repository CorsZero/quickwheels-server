using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Admin.DeleteUserAdmin;

[ApiController]
[Route("api/admin/users")]
public class DeleteUserAdminController : ControllerBase
{
    private readonly DeleteUserAdminHandler _handler;

    public DeleteUserAdminController(DeleteUserAdminHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var currentUserId = HttpContext.Items["UserId"] as Guid?;
        if (!currentUserId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            await _handler.Handle(userId, currentUserId.Value);
            return Ok(ApiResponse.SuccessResult(message: "User deleted successfully"));
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
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while deleting user"));
        }
    }
}
