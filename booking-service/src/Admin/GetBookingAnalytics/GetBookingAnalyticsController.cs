using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Middlewares;
using booking_service.Shared.Types;

namespace booking_service.Admin.GetBookingAnalytics;

[ApiController]
[Route("api/admin/bookings")]
public class GetBookingAnalyticsController : ControllerBase
{
    private readonly GetBookingAnalyticsHandler _handler;

    public GetBookingAnalyticsController(GetBookingAnalyticsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(startDate, endDate);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching analytics"));
        }
    }
}
