namespace LingoHop.Application.Common;

/// <summary>
/// Transport-agnostic failure category. The API layer is the only place that knows
/// how these map onto HTTP status codes.
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
}
