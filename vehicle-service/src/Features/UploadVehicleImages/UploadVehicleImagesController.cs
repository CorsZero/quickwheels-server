using Microsoft.AspNetCore.Mvc;
using vehicle_service.Shared.Types;

namespace vehicle_service.Features.UploadVehicleImages;

[ApiController]
[Route("api/vehicles")]
public class UploadVehicleImagesController : ControllerBase
{
    private readonly UploadVehicleImagesHandler _handler;

    public UploadVehicleImagesController(UploadVehicleImagesHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Upload vehicle images to S3 storage.
    /// Returns object keys to be stored with the vehicle record.
    /// </summary>
    [HttpPost("images")]
    [RequestSizeLimit(52_428_800)] // 50MB total request limit
    public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (!userId.HasValue)
            return Unauthorized(ApiResponse.ErrorResult("Unauthorized"));

        try
        {
            var result = await _handler.Handle(images, userId.Value);

            if (result.Errors.Count > 0 && result.UploadedCount > 0)
            {
                // Partial success
                return Ok(ApiResponse.SuccessResult(new
                {
                    objectKeys = result.ObjectKeys,
                    uploadedCount = result.UploadedCount,
                    errors = result.Errors
                }, "Some images uploaded successfully with errors"));
            }

            return Ok(ApiResponse.SuccessResult(new
            {
                objectKeys = result.ObjectKeys,
                uploadedCount = result.UploadedCount
            }, "Images uploaded successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.ErrorResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while uploading images"));
        }
    }
}
