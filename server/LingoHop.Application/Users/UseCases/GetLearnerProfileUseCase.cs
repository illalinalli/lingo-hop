using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Users.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Users.UseCases;

/// <summary>
/// Loads the current learner, registering them on first launch. This is the endpoint the
/// mini app calls on start-up, so it doubles as sign-up.
/// </summary>
public sealed class GetLearnerProfileUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<Result<LearnerProfileDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var deckCount = await decks.CountByOwnerAsync(user.Id, cancellationToken);

        // Persists the refreshed Telegram display name, if it changed.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDto(clock.Today, deckCount);
    }
}
