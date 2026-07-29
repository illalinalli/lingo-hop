using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>Adds a card to one of the learner's decks.</summary>
public sealed class AddCardUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<Result<CardDto>> ExecuteAsync(
        Guid deckId,
        CardDraft draft,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deck = await decks.FindByIdAsync(deckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(deckId);
        }

        var partOfSpeech = PartOfSpeechParser.Parse(draft.PartOfSpeech);
        if (!partOfSpeech.IsSuccess)
        {
            return partOfSpeech.Error;
        }

        var card = deck.AddCard(
            Term.Create(draft.Term),
            Translation.Create(draft.Translation),
            partOfSpeech.Value,
            ExampleSentence.CreateOrNull(draft.Example),
            clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return card.ToDto();
    }
}
