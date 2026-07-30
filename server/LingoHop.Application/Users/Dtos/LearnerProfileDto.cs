namespace LingoHop.Application.Users.Dtos;

/// <summary>
/// Everything the mini app header and reward sheet need about the current learner.
/// </summary>
public sealed record LearnerProfileDto
{
    public required Guid Id { get; init; }

    public required long TelegramId { get; init; }

    public required string DisplayName { get; init; }

    public required string FirstName { get; init; }

    public string? Username { get; init; }

    public string? LanguageCode { get; init; }

    /// <summary>Level derived from XP, starting at 1.</summary>
    public required int Level { get; init; }

    public required int Experience { get; init; }

    public required int ExperienceIntoLevel { get; init; }

    public required int ExperiencePerLevel { get; init; }

    /// <summary>Progress through the current level, 0..1.</summary>
    public required double LevelProgress { get; init; }

    /// <summary>Consecutive study days as of today - the 🔥 counter.</summary>
    public required int Streak { get; init; }

    public required int LongestStreak { get; init; }

    public required int DailyGoalCards { get; init; }

    public required int CardsReviewedToday { get; init; }

    public required bool DailyGoalCompleted { get; init; }

    /// <summary>
    /// XP earned today that the daily goal has not released yet. It is added to
    /// <see cref="Experience"/> the moment today's goal is reached.
    /// </summary>
    public required int PendingExperience { get; init; }

    public required int DeckCount { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}
