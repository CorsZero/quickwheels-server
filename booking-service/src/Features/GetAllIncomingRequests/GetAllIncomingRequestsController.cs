using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.GetAllIncomingRequests;

[ApiController]
[Route("api/bookings")]
public class GetAllIncomingRequestsController : ControllerBase
{
    private readonly GetAllIncomingRequestsHandler _handler;

    public GetAllIncomingRequestsController(GetAllIncomingRequestsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("all-requests")]
    public async Task<IActionResult> GetAllIncomingRequests([FromQuery] GetAllIncomingRequestsRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(request);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching all booking requests"));
        }
    }
}
