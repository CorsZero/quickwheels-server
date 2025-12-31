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
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds);
            
            if (result == null)
                return NotFound(ApiResponse.ErrorResult("Booking not found"));

            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ApiResponse.ErrorResult(ex.Message));
        }
        catch (InvalidOperationException ex) {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while approving the booking"));
        }
    }
}
