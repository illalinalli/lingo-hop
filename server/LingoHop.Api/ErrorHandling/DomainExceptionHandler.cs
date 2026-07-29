using LingoHop.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.ErrorHandling;

/// <summary>
/// Turns a broken domain invariant into a 400 ProblemDetails response. Invariant violations
/// are programming/user-input errors the Application layer deliberately does not pre-check,
/// so this is the single place they become HTTP.
/// </summary>
internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = domainException,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The request violates a rule of the learning model.",
                Detail = domainException.Message,
                Extensions = { ["code"] = "domain.invariant_violation" },
            },
        });
    }
}
