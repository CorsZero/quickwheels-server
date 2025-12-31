using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Middlewares;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.ActivateVehicle;

[ApiController]
[Route("api/admin/vehicles")]
public class ActivateVehicleController : ControllerBase
{
    private readonly ActivateVehicleHandler _handler;

    public ActivateVehicleController(ActivateVehicleHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{vehicleId:guid}/activate")]
    public async Task<IActionResult> ActivateVehicle(Guid vehicleId)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(vehicleId);
            return Ok(ApiResponse.SuccessResult(result, "Vehicle activated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while activating the vehicle"));
        }
    }
}
