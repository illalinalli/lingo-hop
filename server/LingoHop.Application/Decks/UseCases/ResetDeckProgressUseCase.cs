using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>Clears the mastery counters of every card so the deck can be learned again.</summary>
public sealed class ResetDeckProgressUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<DeckDetailsDto>> ExecuteAsync(
        Guid deckId,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(deckId);
        }

        deck.ResetProgress();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return deck.ToDetailsDto();
    }
}
