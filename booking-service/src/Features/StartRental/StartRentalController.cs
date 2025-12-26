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
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds);
            
            if (result == null)
                return NotFound(ApiResponse.ErrorResult("Booking not found"));

            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex) {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while starting the rental"));
        }
    }
}
