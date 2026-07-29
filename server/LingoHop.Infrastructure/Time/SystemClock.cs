using LingoHop.Application.Abstractions;

namespace LingoHop.Infrastructure.Time;

/// <summary>Wall clock in UTC. Streaks and daily goals use the UTC calendar day.</summary>
internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
