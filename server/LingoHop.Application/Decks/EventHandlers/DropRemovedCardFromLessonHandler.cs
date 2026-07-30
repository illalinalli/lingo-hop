using LingoHop.Application.Abstractions;
using LingoHop.Application.Abstractions.Events;
using LingoHop.Domain.Decks.Events;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Decks.EventHandlers;

/// <summary>
/// Keeps a running lesson consistent with the deck it draws from: a card deleted while the
/// lesson is open is dropped from its queue. Otherwise the lesson would sit on a card that
/// can no longer be rendered - never finishing, never paying out, and blocking every later
/// attempt to study that deck, because starting one resumes the unfinished session.
/// Runs inside the same unit of work as the deletion.
/// </summary>
internal sealed class DropRemovedCardFromLessonHandler(IStudySessionRepository sessions, IClock clock)
    : IDomainEventHandler<CardRemovedFromDeckDomainEvent>
{
    public async Task HandleAsync(
        CardRemovedFromDeckDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Starting a lesson resumes the unfinished one, so a deck has at most one.
        var session = await sessions.FindActiveAsync(
            domainEvent.OwnerId,
            domainEvent.DeckId,
            cancellationToken);

        session?.DiscardCard(domainEvent.CardId, clock.UtcNow);
    }
}
