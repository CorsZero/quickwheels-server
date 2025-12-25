using Microsoft.AspNetCore.Mvc;
using booking_service.Shared.Types;

namespace booking_service.Features.CompleteRental;

[ApiController]
[Route("api/bookings")]
public class CompleteRentalController : ControllerBase
{
    private readonly CompleteRentalHandler _handler;

    public CompleteRentalController(CompleteRentalHandler handler)
    {
        _handler = handler;
    }

    [HttpPatch("{bookingId}/complete")]
    public async Task<IActionResult> CompleteRental(Guid bookingId, [FromBody] CompleteRentalRequest request)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.Error("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds);
            
            if (result == null)
                return NotFound(ApiResponse.Error("Booking not found"));

            return Ok(ApiResponse.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error("An error occurred while completing the rental"));
        }
    }
}
