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

    public void ChangeDailyGoal(int cardsPerDay, DateOnly today)
    {
        DomainException.Require(cardsPerDay is > 0 and <= 200, "Daily goal must be between 1 and 200 cards.");
        DailyGoalCards = cardsPerDay;

        // Lowering the goal can settle it there and then, so waiting XP must not get stuck.
        CollectPendingExperienceIfGoalMet(today);
    }

    /// <summary>
    /// Applied when a study session is completed: advances the streak, adds to today's card
    /// count and banks the lesson's XP.
    /// </summary>
    /// <remarks>
    /// The daily goal is what XP rewards - not the individual lesson. Everything earned today
    /// is held in <see cref="DailyProgress"/> and only credited once the goal is reached, so
    /// hopping from deck to deck cannot collect XP without finishing the day's target.
    /// </remarks>
    public void RegisterCompletedSession(int reviewedCards, int experienceEarned, DateOnly today)
    {
        DomainException.Require(reviewedCards >= 0, "Reviewed card count cannot be negative.");

        Streak = Streak.Register(today);
        DailyProgress = DailyProgress.Register(today, reviewedCards, experienceEarned);

        CollectPendingExperienceIfGoalMet(today);
    }

    public bool IsDailyGoalCompletedOn(DateOnly today) =>
        DailyProgress.CardsReviewedOn(today) >= DailyGoalCards;

    /// <summary>XP earned today that the daily goal has not released yet.</summary>
    public int PendingExperienceOn(DateOnly today) => DailyProgress.PendingExperienceOn(today);

    private void CollectPendingExperienceIfGoalMet(DateOnly today)
    {
        if (!IsDailyGoalCompletedOn(today) || DailyProgress.PendingExperienceOn(today) == 0)
        {
            return;
        }

        Experience = Experience.Add(DailyProgress.PendingExperience);
        DailyProgress = DailyProgress.WithoutPendingExperience();
    }
}
