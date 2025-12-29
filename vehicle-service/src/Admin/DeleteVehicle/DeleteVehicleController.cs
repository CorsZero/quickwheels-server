using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Middlewares;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.DeleteVehicle;

[ApiController]
[Route("api/admin/vehicles")]
public class DeleteVehicleController : ControllerBase
{
    private readonly DeleteVehicleHandler _handler;

    public DeleteVehicleController(DeleteVehicleHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{vehicleId:guid}")]
    public async Task<IActionResult> DeleteVehicle(Guid vehicleId)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            await _handler.Handle(vehicleId);
            return Ok(ApiResponse.SuccessResult(message: "Vehicle deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while deleting the vehicle"));
        }
    }
}
