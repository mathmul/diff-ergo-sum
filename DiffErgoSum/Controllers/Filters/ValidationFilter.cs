namespace DiffErgoSum.Controllers.Filters;

using DiffErgoSum.Controllers.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


/// <summary>
/// Converts model validation errors into standardized <see cref="ApiErrorResponse"/> responses.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = context.ModelState
            .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToArray();

        context.Result = new BadRequestObjectResult(new ApiErrorResponse(
            "ValidationError",
            errors.Length > 0 ? string.Join("; ", errors) : "Invalid request body."
        ));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
