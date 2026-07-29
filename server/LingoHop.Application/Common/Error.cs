namespace LingoHop.Application.Common;

/// <summary>A machine-readable failure returned by a use case.</summary>
/// <param name="Code">Stable dotted identifier, e.g. <c>deck.not_found</c>.</param>
/// <param name="Message">Human readable text safe to show in the mini app.</param>
/// <param name="Type">Category that decides the HTTP status code.</param>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);

    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
}
