using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Domain.Study;

namespace LingoHop.Application.Study.UseCases;

/// <summary>
/// Drops an unfinished lesson without a reward, so the next "Start" builds a fresh queue.
/// </summary>
public sealed class AbandonStudySessionUseCase(
    ICurrentLearner currentLearner,
    IStudySessionRepository sessions,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<Result> ExecuteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);

        var session = await sessions.FindByIdAsync(sessionId, cancellationToken);
        if (session is null || !session.IsOwnedBy(user.Id))
        {
            return Result.Failure(StudyErrors.SessionNotFound(sessionId));
        }

        session.Abandon(clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
