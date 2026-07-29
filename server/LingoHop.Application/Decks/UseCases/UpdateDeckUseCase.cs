using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <param name="Title">New deck name.</param>
/// <param name="Icon">New emoji badge; omit to keep the current one.</param>
public sealed record UpdateDeckCommand(string Title, string? Icon = null);

/// <summary>Renames a deck and/or changes its emoji.</summary>
public sealed class UpdateDeckUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<DeckDetailsDto>> ExecuteAsync(
        Guid deckId,
        UpdateDeckCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(deckId);
        }

        deck.Rename(DeckTitle.Create(command.Title));
        if (command.Icon is not null)
        {
            deck.ChangeIcon(DeckIcon.Create(command.Icon));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return deck.ToDetailsDto();
    }
}
