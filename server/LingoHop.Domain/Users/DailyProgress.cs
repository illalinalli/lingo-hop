using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Cards reviewed on a single calendar day - backs the "Daily goal completed" badge - together
/// with the XP those cards earned but that has not been paid out yet. XP is only credited when
/// the daily goal is reached, so the day's earnings wait here until then.
/// Rolls over automatically the first time activity is recorded on a new day.
/// </summary>
public sealed record DailyProgress
{
    private DailyProgress(DateOnly? date, int cardsReviewed, int pendingExperience)
    {
        Date = date;
        CardsReviewed = cardsReviewed;
        PendingExperience = pendingExperience;
    }

    public DateOnly? Date { get; }

    public int CardsReviewed { get; }

    /// <summary>XP earned today that is still waiting for the daily goal to be reached.</summary>
    public int PendingExperience { get; }

    public static DailyProgress None() => new(null, 0, 0);

    public DailyProgress Register(DateOnly today, int cardsReviewed, int experienceEarned)
    {
        DomainException.Require(cardsReviewed >= 0, "Reviewed card count cannot be negative.");
        DomainException.Require(experienceEarned >= 0, "Earned experience cannot be negative.");

        return Date == today
            ? new DailyProgress(today, CardsReviewed + cardsReviewed, PendingExperience + experienceEarned)
            : new DailyProgress(today, cardsReviewed, experienceEarned);
    }

    /// <summary>Empties the waiting bucket, once its XP has been credited to the learner.</summary>
    public DailyProgress WithoutPendingExperience() => new(Date, CardsReviewed, 0);

    /// <summary>Cards reviewed on <paramref name="today"/>; 0 once the stored day is stale.</summary>
    public int CardsReviewedOn(DateOnly today) => Date == today ? CardsReviewed : 0;

    /// <summary>XP waiting on <paramref name="today"/>; 0 once the stored day is stale.</summary>
    public int PendingExperienceOn(DateOnly today) => Date == today ? PendingExperience : 0;
}
