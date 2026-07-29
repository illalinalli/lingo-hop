using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// The "🔥 7" counter from the design: consecutive calendar days with at least one finished session.
/// </summary>
public sealed record StudyStreak
{
    private StudyStreak(int current, int longest, DateOnly? lastStudiedOn)
    {
        Current = current;
        Longest = longest;
        LastStudiedOn = lastStudiedOn;
    }

    public int Current { get; }

    public int Longest { get; }

    public DateOnly? LastStudiedOn { get; }

    public static StudyStreak None() => new(0, 0, null);

    /// <summary>
    /// Records study activity on <paramref name="today"/> and returns the resulting streak.
    /// Studying twice on the same day does not advance it; skipping a day resets it to 1.
    /// </summary>
    public StudyStreak Register(DateOnly today)
    {
        if (LastStudiedOn == today)
        {
            return this;
        }

        var current = LastStudiedOn == today.AddDays(-1) ? Current + 1 : 1;
        return new StudyStreak(current, Math.Max(current, Longest), today);
    }

    /// <summary>The streak as it should be displayed on <paramref name="today"/>: a missed day shows 0.</summary>
    public int CurrentOn(DateOnly today) =>
        LastStudiedOn is null || LastStudiedOn < today.AddDays(-1) ? 0 : Current;
}
