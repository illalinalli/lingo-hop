using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>Removes a card from a deck.</summary>
public sealed class DeleteCardUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        Guid deckId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return Result.Failure(DeckErrors.NotFound(deckId));
        }

        if (deck.FindCard(cardId) is null)
        {
            return Result.Failure(DeckErrors.CardNotFound(cardId));
        }

        deck.RemoveCard(cardId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
