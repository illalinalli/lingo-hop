using LingoHop.Application.Abstractions.Events;
using LingoHop.Domain.Study.Events;
using LingoHop.Domain.Users;

namespace LingoHop.Application.Study.EventHandlers;

/// <summary>
/// Applies the reward of a finished lesson to the learner: XP, streak and today's card count.
/// The session aggregate never touches the user aggregate directly - it just says what happened.
/// </summary>
internal sealed class AwardSessionRewardHandler(IUserRepository users)
    : IDomainEventHandler<StudySessionCompletedDomainEvent>
{
    public async Task HandleAsync(
        StudySessionCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(domainEvent.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        user.RegisterCompletedSession(
            domainEvent.AnsweredCards,
            domainEvent.ExperienceEarned,
            DateOnly.FromDateTime(domainEvent.OccurredOnUtc.UtcDateTime));
    }
}
