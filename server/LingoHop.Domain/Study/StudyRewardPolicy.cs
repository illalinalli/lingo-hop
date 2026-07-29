namespace LingoHop.Domain.Study;

/// <summary>
/// The single place that decides how much XP a lesson is worth.
/// Kept as an explicit policy so the reward curve is easy to find and re-tune.
/// </summary>
public static class StudyRewardPolicy
{
    /// <summary>XP for showing up: awarded for every answered card, right or wrong.</summary>
    public const int ExperiencePerAnsweredCard = 10;

    /// <summary>Extra XP for each card the learner actually knew.</summary>
    public const int ExperienceBonusPerKnownCard = 5;

    public static int CalculateExperience(int answeredCards, int knownCards) =>
        (answeredCards * ExperiencePerAnsweredCard) + (knownCards * ExperienceBonusPerKnownCard);
}
