using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Middlewares;
using booking_service.Shared.Types;

namespace booking_service.Admin.GetAllBookingsAdmin;

[ApiController]
[Route("api/admin/bookings")]
public class GetAllBookingsAdminController : ControllerBase
{
    private readonly GetAllBookingsAdminHandler _handler;

    public GetAllBookingsAdminController(GetAllBookingsAdminHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        if (!HttpContext.IsAdmin())
            return StatusCode(403, ApiResponse.ErrorResult("Admin access required"));

        try
        {
            var result = await _handler.Handle(status, page, limit);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching bookings"));
        }
    }
}
