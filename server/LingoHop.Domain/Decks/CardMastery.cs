using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>
/// How well a single card is known. This is the minimal-viable spaced repetition:
/// a card becomes "learned" after <see cref="StreakToLearn"/> consecutive "I know it"
/// answers and drops back the moment the learner misses it.
/// </summary>
public sealed record CardMastery
{
    /// <summary>Consecutive correct answers needed before a card counts as learned.</summary>
    public const int StreakToLearn = 3;

    private CardMastery(int timesSeen, int timesKnown, int correctStreak, DateTimeOffset? lastReviewedAtUtc)
    {
        TimesSeen = timesSeen;
        TimesKnown = timesKnown;
        CorrectStreak = correctStreak;
        LastReviewedAtUtc = lastReviewedAtUtc;
    }

    public int TimesSeen { get; }

    public int TimesKnown { get; }

    public int CorrectStreak { get; }

    public DateTimeOffset? LastReviewedAtUtc { get; }

    public bool IsLearned => CorrectStreak >= StreakToLearn;

    public bool IsNew => TimesSeen == 0;

    /// <summary>Share of correct answers, 0..1.</summary>
    public double Accuracy => TimesSeen == 0 ? 0d : (double)TimesKnown / TimesSeen;

    public static CardMastery New() => new(0, 0, 0, null);

    public CardMastery RegisterKnown(DateTimeOffset reviewedAtUtc) =>
        new(TimesSeen + 1, TimesKnown + 1, CorrectStreak + 1, reviewedAtUtc);

    public CardMastery RegisterUnknown(DateTimeOffset reviewedAtUtc) =>
        new(TimesSeen + 1, TimesKnown, 0, reviewedAtUtc);

    /// <summary>
    /// Priority used when building a session: new cards first, then the weakest ones.
    /// Lower value means "show me sooner".
    /// </summary>
    public int SessionPriority => IsNew ? 0 : IsLearned ? 2 : 1;
}
