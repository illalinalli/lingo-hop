using LingoHop.Domain.Common;

namespace LingoHop.Domain.Study.Events;

/// <summary>
/// Raised when a lesson finishes. A handler applies the reward to the User aggregate
/// (XP, streak, daily goal) inside the same transaction.
/// </summary>
public sealed record StudySessionCompletedDomainEvent(
    Guid SessionId,
    Guid UserId,
    Guid DeckId,
    int TotalCards,
    int AnsweredCards,
    int KnownCards,
    int ExperienceEarned,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
