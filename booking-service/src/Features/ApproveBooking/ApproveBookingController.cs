using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.ApproveBooking;

[ApiController]
[Route("api/bookings")]
public class ApproveBookingController : ControllerBase
{
    private readonly ApproveBookingHandler _handler;

    public ApproveBookingController(ApproveBookingHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId}/approve")]
    public async Task<IActionResult> ApproveBooking(Guid bookingId, [FromBody] ApproveBookingRequest request)
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
            return StatusCode(500, ApiResponse.Error("An error occurred while approving the booking"));
        }
    }
}
