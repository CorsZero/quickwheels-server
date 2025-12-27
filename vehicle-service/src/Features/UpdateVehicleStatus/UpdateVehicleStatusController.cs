using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.UpdateVehicleStatus;

[ApiController]
[Route("api/vehicles")]
public class UpdateVehicleStatusController : ControllerBase
{
    private readonly UpdateVehicleStatusHandler _handler;

    public UpdateVehicleStatusController(UpdateVehicleStatusHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{vehicleId:guid}/status")]
    public async Task<IActionResult> UpdateVehicleStatus(Guid vehicleId, [FromBody] UpdateVehicleStatusRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(vehicleId, request, userId.Value);
            return Ok(ApiResponse.SuccessResult(result, "Vehicle status updated"));
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while updating vehicle status"));
        }
    }
}
