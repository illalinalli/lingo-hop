using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks;
using LingoHop.Application.Study.Dtos;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study.UseCases;

/// <param name="DeckId">Deck to study.</param>
/// <param name="CardLimit">
/// How many cards to queue. Defaults to the whole deck, capped at
/// <see cref="StartStudySessionUseCase.MaxCardsPerSession"/>.
/// </param>
public sealed record StartStudySessionCommand(Guid DeckId, int? CardLimit = null);

/// <summary>
/// Opens a lesson. If the learner already has an unfinished lesson for this deck it is
/// resumed instead of starting a second one, so closing the mini app mid-lesson is safe.
/// </summary>
public sealed class StartStudySessionUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IStudySessionRepository sessions,
    ICardShuffler shuffler,
    IUnitOfWork unitOfWork,
    IClock clock,
    StudySessionStateAssembler assembler)
{
    /// <summary>Keeps one lesson short enough to finish in a sitting.</summary>
    public const int MaxCardsPerSession = 50;

    public async Task<Result<StudySessionStateDto>> ExecuteAsync(
        StartStudySessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);

        var deck = await decks.FindByIdAsync(command.DeckId, cancellationToken);
        if (deck is null || !deck.IsOwnedBy(user.Id))
        {
            return DeckErrors.NotFound(command.DeckId);
        }

        if (deck.CardCount == 0)
        {
            return DeckErrors.Empty(deck.Id);
        }

        var active = await sessions.FindActiveAsync(user.Id, deck.Id, cancellationToken);
        if (active is not null)
        {
            return await assembler.BuildAsync(active, deck, user, cancellationToken);
        }

        var limit = Math.Clamp(command.CardLimit ?? deck.CardCount, 1, MaxCardsPerSession);
        var queue = deck.SelectCardsForStudy(limit, shuffler.Shuffle);

        var session = StudySession.Start(
            user.Id,
            deck.Id,
            [.. queue.Select(card => card.Id)],
            clock.UtcNow);

        sessions.Add(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await assembler.BuildAsync(session, deck, user, cancellationToken);
    }
}
