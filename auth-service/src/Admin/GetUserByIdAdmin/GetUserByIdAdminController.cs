using Microsoft.AspNetCore.Mvc;
using sevaLK_service_auth.Shared.Middlewares;

namespace sevaLK_service_auth.Admin.GetUserByIdAdmin;

[ApiController]
[Route("api/admin/users")]
public class GetUserByIdAdminController : ControllerBase
{
    private readonly GetUserByIdAdminHandler _handler;

    public GetUserByIdAdminController(GetUserByIdAdminHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var currentUserId = HttpContext.Items["UserId"] as Guid?;
        if (!currentUserId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(userId);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching user"));
        }
    }
}
