namespace LingoHop.Domain.Common;

/// <summary>
/// Thrown when an attempt is made to put the domain model into an invalid state.
/// The API layer translates it into a 400 ProblemDetails response.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>Guards an invariant, throwing when <paramref name="condition"/> does not hold.</summary>
    public static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new DomainException(message);
        }
    }
}
