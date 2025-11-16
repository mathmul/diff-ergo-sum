namespace DiffErgoSum.Api.Filters;

using DiffErgoSum.Api.Exceptions;
using DiffErgoSum.Api.Models;
using DiffErgoSum.Core.Constants;

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
            .Values
            .SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToArray();

        // 422: semantic Base64 error
        if (errors.Contains(ValidationMessages.InvalidBase64))
        {
            var ex = new InvalidBase64HttpException();

            context.Result = new ObjectResult(ex.ToProblemDetails(context.HttpContext.Request.Path))
            {
                StatusCode = ex.StatusCode
            };

            return;
        }

        // 400: standard validation error
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
