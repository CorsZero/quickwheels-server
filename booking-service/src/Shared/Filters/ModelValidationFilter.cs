using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using booking_service.Shared.Types;

namespace booking_service.Shared.Filters;

public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => 
                    string.IsNullOrEmpty(e.ErrorMessage) 
                        ? "Invalid input format" 
                        : e.ErrorMessage))
                .ToList();

            var response = ApiResponse.ErrorResult(
                "Validation failed. Please check your input.",
                errors
            );

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
