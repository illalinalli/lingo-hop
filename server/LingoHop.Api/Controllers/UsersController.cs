using LingoHop.Application.Users.Dtos;
using LingoHop.Application.Users.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.Controllers;

/// <summary>The learner behind the current Telegram launch payload.</summary>
[Route("api/users")]
[Tags("Users")]
public sealed class UsersController(
    GetLearnerProfileUseCase getLearnerProfile,
    UpdateDailyGoalUseCase updateDailyGoal) : ApiControllerBase
{
    /// <summary>
    /// Returns the current learner, registering them on the first launch of the mini app.
    /// This is the call the client makes on start-up.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType<LearnerProfileDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<LearnerProfileDto>> GetMe(CancellationToken cancellationToken) =>
        Respond(await getLearnerProfile.ExecuteAsync(cancellationToken));

    /// <summary>Changes how many cards a day count as the daily goal.</summary>
    [HttpPut("me/daily-goal")]
    [ProducesResponseType<LearnerProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LearnerProfileDto>> UpdateDailyGoal(
        UpdateDailyGoalCommand command,
        CancellationToken cancellationToken) =>
        Respond(await updateDailyGoal.ExecuteAsync(command, cancellationToken));
}
