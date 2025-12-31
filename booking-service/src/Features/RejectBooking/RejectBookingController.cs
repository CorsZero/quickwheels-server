using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.RejectBooking;

[ApiController]
[Route("api/bookings")]
public class RejectBookingController : ControllerBase
{
    private readonly RejectBookingHandler _handler;

    public RejectBookingController(RejectBookingHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId}/reject")]
    public async Task<IActionResult> RejectBooking(Guid bookingId, [FromBody] RejectBookingRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds, request.Reason);
            
            if (result == null)
                return NotFound(ApiResponse.ErrorResult("Booking not found"));

            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ApiResponse.ErrorResult(ex.Message));
        }
        catch (ArgumentException ex) {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (InvalidOperationException ex) {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while rejecting the booking"));
        }
    }
}
