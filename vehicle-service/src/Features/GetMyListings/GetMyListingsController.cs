using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.GetMyListings;

[ApiController]
[Route("api/vehicles")]
public class GetMyListingsController : ControllerBase
{
    private readonly GetMyListingsHandler _handler;

    public GetMyListingsController(GetMyListingsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("my-listings")]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(userId.Value, page, limit);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching your listings"));
        }
    }
}
