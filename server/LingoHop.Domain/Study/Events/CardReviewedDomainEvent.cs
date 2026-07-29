using LingoHop.Domain.Common;

namespace LingoHop.Domain.Study.Events;

/// <summary>
/// Raised by <see cref="StudySession"/> when a card is graded. The Deck aggregate lives
/// behind its own boundary, so its mastery counters are updated by a handler reacting to
/// this event rather than by the session reaching across aggregates.
/// </summary>
public sealed record CardReviewedDomainEvent(
    Guid SessionId,
    Guid DeckId,
    Guid CardId,
    bool Known,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
