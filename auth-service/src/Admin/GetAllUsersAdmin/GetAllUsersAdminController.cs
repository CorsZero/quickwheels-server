using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Admin.GetAllUsersAdmin;

[ApiController]
[Route("api/admin/users")]
public class GetAllUsersAdminController : ControllerBase
{
    private readonly GetAllUsersAdminHandler _handler;

    public GetAllUsersAdminController(GetAllUsersAdminHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(page, limit, search, role);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching users"));
        }
    }
}
