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
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(bookingId, userId.Value, request.OwnerVehicleIds);
            
            if (result == null)
                return NotFound(ApiResponse.ErrorResult("Booking not found"));

            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ApiResponse.ErrorResult(ex.Message));
        }
        catch (InvalidOperationException ex) {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while completing the rental"));
        }
    }
}
