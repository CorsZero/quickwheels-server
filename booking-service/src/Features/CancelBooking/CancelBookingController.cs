using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.CancelBooking;

[ApiController]
[Route("api/bookings")]
public class CancelBookingController : ControllerBase
{
    private readonly CancelBookingHandler _handler;

    public CancelBookingController(CancelBookingHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.Error("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value);
            
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
            return StatusCode(500, ApiResponse.Error("An error occurred while cancelling the booking"));
        }
    }
}
