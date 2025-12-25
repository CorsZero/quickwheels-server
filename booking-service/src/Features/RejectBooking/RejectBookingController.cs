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
            return Unauthorized(ApiResponse.Error("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds, request.Reason);
            
            if (result == null)
                return NotFound(ApiResponse.Error("Booking not found"));

            return Ok(ApiResponse.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error("An error occurred while rejecting the booking"));
        }
    }
}
