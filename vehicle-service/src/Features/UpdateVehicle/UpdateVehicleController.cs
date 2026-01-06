using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.UpdateVehicle;

[ApiController]
[Route("api/vehicles")]
public class UpdateVehicleController : ControllerBase
{
    private readonly UpdateVehicleHandler _handler;

    public UpdateVehicleController(UpdateVehicleHandler handler)
    {
        _handler = handler;
    }

    [HttpPut("{vehicleId:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid vehicleId, [FromForm] UpdateVehicleRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(vehicleId, request, userId.Value);
            return Ok(ApiResponse.SuccessResult(result, "Vehicle updated successfully"));
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
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while updating the vehicle"));
        }
    }
}
