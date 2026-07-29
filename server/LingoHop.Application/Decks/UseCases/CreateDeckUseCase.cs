using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <param name="Title">Deck name, up to 80 characters.</param>
/// <param name="Icon">Optional emoji badge; a book emoji is used when omitted.</param>
/// <param name="Cards">Optional cards to add straight away, so a deck can be created in one call.</param>
public sealed record CreateDeckCommand(
    string Title,
    string? Icon = null,
    IReadOnlyList<CardDraft>? Cards = null);

/// <summary>Creates a deck for the current learner, optionally pre-filled with cards.</summary>
public sealed class CreateDeckUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<Result<DeckDetailsDto>> ExecuteAsync(
        CreateDeckCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);

        var deck = Deck.Create(
            user.Id,
            DeckTitle.Create(command.Title),
            DeckIcon.Create(command.Icon),
            clock.UtcNow);

        foreach (var draft in command.Cards ?? [])
        {
            var partOfSpeech = PartOfSpeechParser.Parse(draft.PartOfSpeech);
            if (!partOfSpeech.IsSuccess)
            {
                return partOfSpeech.Error;
            }

            deck.AddCard(
                Term.Create(draft.Term),
                Translation.Create(draft.Translation),
                partOfSpeech.Value,
                ExampleSentence.CreateOrNull(draft.Example),
                clock.UtcNow);
        }

        decks.Add(deck);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return deck.ToDetailsDto();
    }
}
