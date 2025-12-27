using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.GetAllVehicles;

[ApiController]
[Route("api/vehicles")]
public class GetAllVehiclesController : ControllerBase
{
    private readonly GetAllVehiclesHandler _handler;

    public GetAllVehiclesController(GetAllVehiclesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicles(
        [FromQuery] string? location,
        [FromQuery] string? district,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? transmission,
        [FromQuery] string? fuelType,
        [FromQuery] int? seats,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 12)
    {
        try
        {
            var result = await _handler.Handle(
                location, district, category, minPrice, maxPrice,
                transmission, fuelType, seats, search, page, limit);
            return Ok(ApiResponse.SuccessResult(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while fetching vehicles"));
        }
    }
}
