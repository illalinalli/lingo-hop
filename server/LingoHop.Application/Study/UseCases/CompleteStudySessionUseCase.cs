using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Study.Dtos;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study.UseCases;

/// <summary>
/// Finishes a lesson before the queue is exhausted, keeping the reward for what was answered.
/// A fully answered lesson completes on its own, so this is only needed for an early exit.
/// </summary>
public sealed class CompleteStudySessionUseCase(
    ICurrentLearner currentLearner,
    IStudySessionRepository sessions,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock,
    StudySessionStateAssembler assembler)
{
    public async Task<Result<StudySessionStateDto>> ExecuteAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);

        var session = await sessions.FindByIdAsync(sessionId, cancellationToken);
        if (session is null || !session.IsOwnedBy(user.Id))
        {
            return StudyErrors.SessionNotFound(sessionId);
        }

        var deck = await decks.FindByIdAsync(session.DeckId, cancellationToken);
        if (deck is null)
        {
            return StudyErrors.DeckGone(session.DeckId);
        }

        session.Complete(clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await assembler.BuildAsync(session, deck, user, cancellationToken);
    }
}
