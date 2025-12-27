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
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value);
            
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
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while cancelling the booking"));
        }
    }
}
