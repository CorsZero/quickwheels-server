using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.CreateVehicle;

[ApiController]
[Route("api/vehicles")]
public class CreateVehicleController : ControllerBase
{
    private readonly CreateVehicleHandler _handler;

    public CreateVehicleController(CreateVehicleHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(request, userId.Value);
            return Created("", ApiResponse.SuccessResult(result, "Vehicle listed successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while creating the vehicle listing"));
        }
    }
}
