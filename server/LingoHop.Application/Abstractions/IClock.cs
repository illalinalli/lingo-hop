namespace LingoHop.Application.Abstractions;

/// <summary>
/// Time as a dependency, so streak and daily-goal behaviour is testable.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }

    /// <summary>Current date in UTC - the calendar used for streaks and daily goals.</summary>
    DateOnly Today { get; }
}
