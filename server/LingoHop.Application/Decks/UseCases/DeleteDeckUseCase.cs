using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>Deletes a deck together with its cards and study history.</summary>
public sealed class DeleteDeckUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(Guid deckId, CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return Result.Failure(DeckErrors.NotFound(deckId));
        }

        decks.Remove(deck);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
