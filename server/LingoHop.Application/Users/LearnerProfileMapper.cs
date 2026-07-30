using LingoHop.Application.Users.Dtos;
using LingoHop.Domain.Users;

namespace LingoHop.Application.Users;

/// <summary>Hand-written projection from the User aggregate to its transport shape.</summary>
internal static class LearnerProfileMapper
{
    public static LearnerProfileDto ToDto(this User user, DateOnly today, int deckCount) => new()
    {
        Id = user.Id,
        TelegramId = user.TelegramId.Value,
        DisplayName = user.Profile.DisplayName,
        FirstName = user.Profile.FirstName,
        Username = user.Profile.Username,
        LanguageCode = user.Profile.LanguageCode,
        Level = user.Experience.Level,
        Experience = user.Experience.Value,
        ExperienceIntoLevel = user.Experience.PointsIntoLevel,
        ExperiencePerLevel = ExperiencePoints.PointsPerLevel,
        LevelProgress = user.Experience.LevelProgress,
        Streak = user.Streak.CurrentOn(today),
        LongestStreak = user.Streak.Longest,
        DailyGoalCards = user.DailyGoalCards,
        CardsReviewedToday = user.DailyProgress.CardsReviewedOn(today),
        DailyGoalCompleted = user.IsDailyGoalCompletedOn(today),
        PendingExperience = user.PendingExperienceOn(today),
        DeckCount = deckCount,
        CreatedAtUtc = user.CreatedAtUtc,
    };
}
