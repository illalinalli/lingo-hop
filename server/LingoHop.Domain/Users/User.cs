using LingoHop.Domain.Common;

namespace LingoHop.Domain.Users;

/// <summary>
/// Aggregate root for a learner. Owns everything that is global to the person:
/// their Telegram identity, XP, streak and daily goal. Decks and sessions are
/// separate aggregates that reference this one by <see cref="Entity.Id"/>.
/// </summary>
public sealed class User : AggregateRoot
{
    /// <summary>Cards per day the mini app asks for out of the box.</summary>
    public const int DefaultDailyGoalCards = 10;

    private User(
        Guid id,
        TelegramUserId telegramId,
        TelegramProfile profile,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        TelegramId = telegramId;
        Profile = profile;
        Experience = ExperiencePoints.Zero();
        Streak = StudyStreak.None();
        DailyProgress = DailyProgress.None();
        DailyGoalCards = DefaultDailyGoalCards;
        CreatedAtUtc = createdAtUtc;
    }

    private User()
    {
        // EF Core materialisation.
        TelegramId = null!;
        Profile = null!;
        Experience = null!;
        Streak = null!;
        DailyProgress = null!;
    }

    public TelegramUserId TelegramId { get; private set; }

    public TelegramProfile Profile { get; private set; }

    public ExperiencePoints Experience { get; private set; }

    public StudyStreak Streak { get; private set; }

    public DailyProgress DailyProgress { get; private set; }

    public int DailyGoalCards { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static User Register(TelegramUserId telegramId, TelegramProfile profile, DateTimeOffset nowUtc) =>
        new(Guid.CreateVersion7(), telegramId, profile, nowUtc);

    /// <summary>Refreshes the cached Telegram display data. Idempotent.</summary>
    public void RefreshProfile(TelegramProfile profile) => Profile = profile;

    public void ChangeDailyGoal(int cardsPerDay)
    {
        DomainException.Require(cardsPerDay is > 0 and <= 200, "Daily goal must be between 1 and 200 cards.");
        DailyGoalCards = cardsPerDay;
    }

    /// <summary>
    /// Applied when a study session is completed: awards XP, advances the streak
    /// and adds to today's card count.
    /// </summary>
    public void RegisterCompletedSession(int reviewedCards, int experienceEarned, DateOnly today)
    {
        DomainException.Require(reviewedCards >= 0, "Reviewed card count cannot be negative.");

        Experience = Experience.Add(experienceEarned);
        Streak = Streak.Register(today);
        DailyProgress = DailyProgress.Register(today, reviewedCards);
    }

    public bool IsDailyGoalCompletedOn(DateOnly today) =>
        DailyProgress.CardsReviewedOn(today) >= DailyGoalCards;
}
