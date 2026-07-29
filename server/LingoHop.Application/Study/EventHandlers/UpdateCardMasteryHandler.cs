using LingoHop.Application.Abstractions.Events;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study.Events;

namespace LingoHop.Application.Study.EventHandlers;

/// <summary>
/// Keeps the Deck aggregate's mastery counters in step with the grades given during a lesson.
/// Runs inside the same unit of work, so the session answer and the card counters commit together.
/// </summary>
internal sealed class UpdateCardMasteryHandler(IDeckRepository decks)
    : IDomainEventHandler<CardReviewedDomainEvent>
{
    public async Task HandleAsync(
        CardReviewedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var deck = await decks.FindByIdAsync(domainEvent.DeckId, cancellationToken);

        // The card may have been edited away between starting and finishing the lesson.
        if (deck?.FindCard(domainEvent.CardId) is null)
        {
            return;
        }

        deck.RegisterReview(domainEvent.CardId, domainEvent.Known, domainEvent.OccurredOnUtc);
    }
}
