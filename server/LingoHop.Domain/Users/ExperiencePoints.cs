using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Accumulated XP. Levels are derived, never stored, so the curve can be re-tuned freely.
/// </summary>
public sealed record ExperiencePoints
{
    /// <summary>XP required to advance one level.</summary>
    public const int PointsPerLevel = 500;

    private ExperiencePoints(int value) => Value = value;

    public int Value { get; }

    /// <summary>Level shown as "Level 4" in the header; starts at 1.</summary>
    public int Level => 1 + (Value / PointsPerLevel);

    /// <summary>XP collected inside the current level.</summary>
    public int PointsIntoLevel => Value % PointsPerLevel;

    /// <summary>How far the user is through the current level, 0..1.</summary>
    public double LevelProgress => (double)PointsIntoLevel / PointsPerLevel;

    public static ExperiencePoints Zero() => new(0);

    public static ExperiencePoints Create(int value)
    {
        DomainException.Require(value >= 0, "Experience points cannot be negative.");
        return new ExperiencePoints(value);
    }

    public ExperiencePoints Add(int points)
    {
        DomainException.Require(points >= 0, "Cannot award a negative amount of experience points.");
        return new ExperiencePoints(Value + points);
    }
}
