using LingoHop.Application.Common;

namespace LingoHop.Application.Decks;

/// <summary>Failures the deck use cases can return, in one place.</summary>
public static class DeckErrors
{
    public static Error NotFound(Guid deckId) =>
        Error.NotFound("deck.not_found", $"Deck {deckId} was not found.");

    public static Error CardNotFound(Guid cardId) =>
        Error.NotFound("card.not_found", $"Card {cardId} was not found.");

    public static Error Empty(Guid deckId) =>
        Error.Validation("deck.empty", $"Deck {deckId} has no cards to study yet.");
}
