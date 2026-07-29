using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>Edits the text of an existing card. Mastery counters are left untouched.</summary>
public sealed class UpdateCardUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CardDto>> ExecuteAsync(
        Guid deckId,
        Guid cardId,
        CardDraft draft,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(deckId);
        }

        if (deck.FindCard(cardId) is null)
        {
            return DeckErrors.CardNotFound(cardId);
        }

        var partOfSpeech = PartOfSpeechParser.Parse(draft.PartOfSpeech);
        if (!partOfSpeech.IsSuccess)
        {
            return partOfSpeech.Error;
        }

        deck.UpdateCard(
            cardId,
            Term.Create(draft.Term),
            Translation.Create(draft.Translation),
            partOfSpeech.Value,
            ExampleSentence.CreateOrNull(draft.Example));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return deck.FindCard(cardId)!.ToDto();
    }
}
