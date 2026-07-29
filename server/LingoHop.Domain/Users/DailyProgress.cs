using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Cards reviewed on a single calendar day - backs the "Daily goal completed" badge.
/// Rolls over automatically the first time activity is recorded on a new day.
/// </summary>
public sealed record DailyProgress
{
    private DailyProgress(DateOnly? date, int cardsReviewed)
    {
        Date = date;
        CardsReviewed = cardsReviewed;
    }

    public DateOnly? Date { get; }

    public int CardsReviewed { get; }

    public static DailyProgress None() => new(null, 0);

    public DailyProgress Register(DateOnly today, int cardsReviewed)
    {
        DomainException.Require(cardsReviewed >= 0, "Reviewed card count cannot be negative.");
        return Date == today
            ? new DailyProgress(today, CardsReviewed + cardsReviewed)
            : new DailyProgress(today, cardsReviewed);
    }

    /// <summary>Cards reviewed on <paramref name="today"/>; 0 once the stored day is stale.</summary>
    public int CardsReviewedOn(DateOnly today) => Date == today ? CardsReviewed : 0;
}
