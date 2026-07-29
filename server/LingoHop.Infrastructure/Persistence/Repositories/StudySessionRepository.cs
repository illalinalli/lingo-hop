using LingoHop.Domain.Study;
using Microsoft.EntityFrameworkCore;

namespace LingoHop.Infrastructure.Persistence.Repositories;

internal sealed class StudySessionRepository(LingoHopDbContext context) : IStudySessionRepository
{
    public Task<StudySession?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        context.StudySessions.FirstOrDefaultAsync(session => session.Id == sessionId, cancellationToken);

    public Task<StudySession?> FindActiveAsync(
        Guid userId,
        Guid deckId,
        CancellationToken cancellationToken = default) =>
        context.StudySessions
            .Where(session =>
                session.UserId == userId &&
                session.DeckId == deckId &&
                session.Status == StudySessionStatus.InProgress)
            .OrderByDescending(session => session.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(StudySession session) => context.StudySessions.Add(session);
}
