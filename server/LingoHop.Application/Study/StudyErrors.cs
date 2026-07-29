using LingoHop.Application.Common;

namespace LingoHop.Application.Study;

/// <summary>Failures the study use cases can return, in one place.</summary>
public static class StudyErrors
{
    public static Error SessionNotFound(Guid sessionId) =>
        Error.NotFound("study_session.not_found", $"Study session {sessionId} was not found.");

    public static Error DeckGone(Guid deckId) =>
        Error.NotFound("study_session.deck_gone", $"The deck {deckId} of this lesson no longer exists.");
}
