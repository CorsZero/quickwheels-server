using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.GetMyRequests;

[ApiController]
[Route("api/bookings")]
public class GetMyRequestsController : ControllerBase
{
    private readonly GetMyRequestsHandler _handler;

    public GetMyRequestsController(GetMyRequestsHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("my-requests")]
    public async Task<IActionResult> GetMyRequests([FromBody] GetMyRequestsRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(request);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching booking requests"));
        }
    }
}
