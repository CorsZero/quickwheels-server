using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.DeleteVehicleImage;

[ApiController]
[Route("api/vehicles")]
public class DeleteVehicleImageController : ControllerBase
{
    private readonly DeleteVehicleImageHandler _handler;

    public DeleteVehicleImageController(DeleteVehicleImageHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{vehicleId:guid}/images")]
    public async Task<IActionResult> DeleteVehicleImage(Guid vehicleId, [FromBody] DeleteVehicleImageRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(vehicleId, userId.Value, request);
            return Ok(ApiResponse.SuccessResult(result, "Image deleted successfully"));
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
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResult($"An error occurred while deleting the image: {ex.Message}"));
        }
    }
}
