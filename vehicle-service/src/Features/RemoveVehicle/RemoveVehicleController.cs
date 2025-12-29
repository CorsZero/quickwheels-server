using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Middlewares;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.RemoveVehicle;

[ApiController]
[Route("api/vehicles")]
public class RemoveVehicleController : ControllerBase
{
    private readonly RemoveVehicleHandler _handler;

    public RemoveVehicleController(RemoveVehicleHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{vehicleId:guid}/remove")]
    public async Task<IActionResult> RemoveVehicle(Guid vehicleId, [FromBody] RemoveVehicleRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(vehicleId, userId.Value, request);
            return Ok(ApiResponse.SuccessResult(result, "Vehicle removed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ApiResponse.ErrorResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while removing the vehicle"));
        }
    }
}
