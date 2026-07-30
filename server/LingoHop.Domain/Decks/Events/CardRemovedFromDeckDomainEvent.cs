using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks.Events;

/// <summary>
/// Raised when a card is taken out of a deck. A running lesson may still have that card in
/// its queue, and the StudySession aggregate lives behind its own boundary - so a handler
/// reacting to this event prunes the queue instead of the deck reaching across aggregates.
/// </summary>
public sealed record CardRemovedFromDeckDomainEvent(
    Guid DeckId,
    Guid OwnerId,
    Guid CardId,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
