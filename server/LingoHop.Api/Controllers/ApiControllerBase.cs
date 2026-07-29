using LingoHop.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.Controllers;

/// <summary>
/// Shared plumbing for the controllers: authorisation and the single place where an
/// Application-layer <see cref="Result"/> becomes an HTTP response.
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Maps a failed result onto the matching ProblemDetails response.</summary>
    protected ActionResult Failure(Error error) => Problem(
        detail: error.Message,
        statusCode: StatusCodeFor(error.Type),
        title: TitleFor(error.Type),
        extensions: new Dictionary<string, object?> { ["code"] = error.Code });

    protected ActionResult<TValue> Respond<TValue>(Result<TValue> result) =>
        result.IsSuccess ? Ok(result.Value) : Failure(result.Error);

    protected ActionResult RespondNoContent(Result result) =>
        result.IsSuccess ? NoContent() : Failure(result.Error);

    private static int StatusCodeFor(ErrorType type) => type switch
    {
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest,
    };

    private static string TitleFor(ErrorType type) => type switch
    {
        ErrorType.NotFound => "Not found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Forbidden => "Forbidden",
        ErrorType.Unauthorized => "Unauthorized",
        _ => "Invalid request",
    };
}
