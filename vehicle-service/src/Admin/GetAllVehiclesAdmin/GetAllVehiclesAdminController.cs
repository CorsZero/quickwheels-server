using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Middlewares;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.GetAllVehiclesAdmin;

[ApiController]
[Route("api/admin/vehicles")]
public class GetAllVehiclesAdminController : ControllerBase
{
    private readonly GetAllVehiclesAdminHandler _handler;

    public GetAllVehiclesAdminController(GetAllVehiclesAdminHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehiclesAdmin(
        [FromQuery] string? status,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(status, isActive, page, limit);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching vehicles"));
        }
    }
}
