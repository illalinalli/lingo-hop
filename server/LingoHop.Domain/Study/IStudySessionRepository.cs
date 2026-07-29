namespace LingoHop.Domain.Study;

/// <summary>Persistence contract for the <see cref="StudySession"/> aggregate.</summary>
public interface IStudySessionRepository
{
    /// <summary>Loads a session together with its queued cards.</summary>
    Task<StudySession?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// The learner's unfinished session for a deck, if any - lets the mini app resume a
    /// lesson that was closed mid-way instead of starting a duplicate one.
    /// </summary>
    Task<StudySession?> FindActiveAsync(Guid userId, Guid deckId, CancellationToken cancellationToken = default);

    void Add(StudySession session);
}
