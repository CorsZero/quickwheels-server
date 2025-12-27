using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.GetMyRentals;

[ApiController]
[Route("api/bookings")]
public class GetMyRentalsController : ControllerBase
{
    private readonly GetMyRentalsHandler _handler;

    public GetMyRentalsController(GetMyRentalsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("my-rentals")]
    public async Task<IActionResult> GetMyRentals([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(userId.Value, status, page, limit);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching rentals"));
        }
    }
}
