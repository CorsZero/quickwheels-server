using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.GetBookingDetails;

[ApiController]
[Route("api/bookings")]
public class GetBookingDetailsController : ControllerBase
{
    private readonly GetBookingDetailsHandler _handler;

    public GetBookingDetailsController(GetBookingDetailsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingDetails(Guid bookingId)
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
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching booking details"));
        }
    }
}
