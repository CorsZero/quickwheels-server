using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.StartRental;

[ApiController]
[Route("api/bookings")]
public class StartRentalController : ControllerBase
{
    private readonly StartRentalHandler _handler;

    public StartRentalController(StartRentalHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId}/start")]
    public async Task<IActionResult> StartRental(Guid bookingId, [FromBody] StartRentalRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.Error("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds);
            
            if (result == null)
                return NotFound(ApiResponse.Error("Booking not found"));

            return Ok(ApiResponse.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error("An error occurred while starting the rental"));
        }
    }
}
