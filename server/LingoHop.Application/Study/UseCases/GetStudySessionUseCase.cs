using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Study.Dtos;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study.UseCases;

/// <summary>Reads a lesson back - used to resume after the mini app was closed.</summary>
public sealed class GetStudySessionUseCase(
    ICurrentLearner currentLearner,
    IStudySessionRepository sessions,
    IDeckRepository decks,
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

        return await assembler.BuildAsync(session, deck, user, cancellationToken);
    }
}
