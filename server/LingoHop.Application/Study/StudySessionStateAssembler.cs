using LingoHop.Application.Abstractions;
using LingoHop.Application.Study.Dtos;
using LingoHop.Application.Users;
using LingoHop.Domain.Decks;
using LingoHop.Domain.Study;
using LingoHop.Domain.Users;

namespace LingoHop.Application.Study;

/// <summary>
/// Builds the response every study endpoint returns: the lesson plus the learner's current
/// stats. Extracted so the five study use cases share one projection.
/// </summary>
public sealed class StudySessionStateAssembler(IDeckRepository decks, IClock clock)
{
    /// <remarks>Call after the unit of work has been committed, so XP and streak are up to date.</remarks>
    public async Task<StudySessionStateDto> BuildAsync(
        StudySession session,
        Deck deck,
        User user,
        CancellationToken cancellationToken = default)
    {
        var deckCount = await decks.CountByOwnerAsync(user.Id, cancellationToken);

        return new StudySessionStateDto
        {
            Session = session.ToDto(deck),
            Learner = user.ToDto(clock.Today, deckCount),
        };
    }
}
