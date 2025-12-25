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
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error("An error occurred while fetching booking details"));
        }
    }
}
