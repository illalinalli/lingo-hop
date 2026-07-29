using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Study.Dtos;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study.UseCases;

/// <param name="CardId">Card being graded.</param>
/// <param name="Known">
/// <c>true</c> for "Know" (swipe right), <c>false</c> for "Don't know" (swipe left).
/// </param>
public sealed record GradeCardCommand(Guid CardId, bool Known);

/// <summary>
/// Records the learner's answer for one card. The session completes itself once the last
/// card in the queue has been graded, which is when XP and the streak are awarded.
/// </summary>
public sealed class GradeCardUseCase(
    ICurrentLearner currentLearner,
    IStudySessionRepository sessions,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock,
    StudySessionStateAssembler assembler)
{
    public async Task<Result<StudySessionStateDto>> ExecuteAsync(
        Guid sessionId,
        GradeCardCommand command,
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

        session.Grade(command.CardId, command.Known, clock.UtcNow);

        // Committing dispatches the domain events: card mastery on the Deck aggregate and,
        // when this was the last card, XP plus streak on the User aggregate.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await assembler.BuildAsync(session, deck, user, cancellationToken);
    }
}
