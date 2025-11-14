namespace DiffErgoSum.Api.Filters;

using DiffErgoSum.Api.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


/// <summary>
/// Converts model validation errors into standardized <see cref="ProblemDetailsResponse"/> responses.
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

        var detail = errors.Length > 0 ? string.Join("; ", errors) : "Invalid request body.";

        var problem = new ProblemDetailsResponse(
            Type: "about:blank",
            Title: "Validation Error",
            Status: StatusCodes.Status400BadRequest,
            Detail: detail,
            Instance: context.HttpContext.Request.Path
        );

        context.Result = new BadRequestObjectResult(problem);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
