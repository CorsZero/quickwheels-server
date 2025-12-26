using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.GetVehicleById;

[ApiController]
[Route("api/vehicles")]
public class GetVehicleByIdController : ControllerBase
{
    private readonly GetVehicleByIdHandler _handler;

    public GetVehicleByIdController(GetVehicleByIdHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{vehicleId:guid}")]
    public async Task<IActionResult> GetVehicleById(Guid vehicleId)
    {
        try
        {
            var result = await _handler.Handle(vehicleId);

            if (result == null)
                return NotFound(ApiResponse.ErrorResult("Vehicle not found"));

            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching the vehicle"));
        }
    }
}
