using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>A single deck with all of its cards.</summary>
public sealed class GetDeckUseCase(ICurrentLearner currentLearner, IDeckRepository decks)
{
    public async Task<Result<DeckDetailsDto>> ExecuteAsync(
        Guid deckId,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);

        // A deck belonging to somebody else is reported as missing, not as forbidden,
        // so the API does not leak which ids exist.
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(deckId);
        }

        return deck.ToDetailsDto();
    }
}
