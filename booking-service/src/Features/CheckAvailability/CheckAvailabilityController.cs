using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.CheckAvailability;

[ApiController]
[Route("api/bookings")]
public class CheckAvailabilityController : ControllerBase
{
    private readonly CheckAvailabilityHandler _handler;

    public CheckAvailabilityController(CheckAvailabilityHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("availability/{vehicleId}")]
    public async Task<IActionResult> CheckAvailability(
        Guid vehicleId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate >= endDate)
            return BadRequest(ApiResponse.Error("Start date must be before end date"));

        if (startDate < DateTime.UtcNow.Date)
            return BadRequest(ApiResponse.Error("Start date cannot be in the past"));

        try
        {
            var result = await _handler.Handle(vehicleId, startDate, endDate);
            return Ok(ApiResponse.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error("An error occurred while checking availability"));
        }
    }
}
