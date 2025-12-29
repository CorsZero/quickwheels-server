using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Middlewares;
using booking_service.Shared.Types;

namespace booking_service.Admin.OverrideBookingStatus;

[ApiController]
[Route("api/admin/bookings")]
public class OverrideBookingStatusController : ControllerBase
{
    private readonly OverrideBookingStatusHandler _handler;

    public OverrideBookingStatusController(OverrideBookingStatusHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId:guid}/status")]
    public async Task<IActionResult> OverrideStatus(
        Guid bookingId,
        [FromBody] OverrideBookingStatusRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(bookingId, request.Status, request.Reason);
            return Ok(ApiResponse.SuccessResult(result, "Booking status updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while updating booking status"));
        }
    }
}

public class OverrideBookingStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
