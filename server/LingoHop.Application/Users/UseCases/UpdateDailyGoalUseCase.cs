using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Users.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Users.UseCases;

/// <param name="CardsPerDay">How many cards a day counts as "goal reached", 1..200.</param>
public sealed record UpdateDailyGoalCommand(int CardsPerDay);

/// <summary>Changes how many cards a day the learner aims for.</summary>
public sealed class UpdateDailyGoalUseCase(
    ICurrentLearner currentLearner,
    IDeckRepository decks,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<Result<LearnerProfileDto>> ExecuteAsync(
        UpdateDailyGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        user.ChangeDailyGoal(command.CardsPerDay);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var deckCount = await decks.CountByOwnerAsync(user.Id, cancellationToken);
        return user.ToDto(clock.Today, deckCount);
    }
}
