using LingoHop.Application.Study.Dtos;
using LingoHop.Application.Study.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.Controllers;

/// <summary>
/// Lessons: the "know / don't know" pass through a deck that drives the progress bar,
/// the XP reward and the streak.
/// </summary>
[Route("api/study-sessions")]
[Tags("Study")]
public sealed class StudySessionsController(
    StartStudySessionUseCase startSession,
    GetStudySessionUseCase getSession,
    GradeCardUseCase gradeCard,
    CompleteStudySessionUseCase completeSession,
    AbandonStudySessionUseCase abandonSession) : ApiControllerBase
{
    /// <summary>
    /// Starts a lesson for a deck, or returns the learner's unfinished lesson for that deck
    /// so a closed mini app can pick up where it left off.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<StudySessionStateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudySessionStateDto>> Start(
        StartStudySessionCommand command,
        CancellationToken cancellationToken) =>
        Respond(await startSession.ExecuteAsync(command, cancellationToken));

    /// <summary>Reads a lesson back, including the grades given so far.</summary>
    [HttpGet("{sessionId:guid}")]
    [ProducesResponseType<StudySessionStateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudySessionStateDto>> Get(
        Guid sessionId,
        CancellationToken cancellationToken) =>
        Respond(await getSession.ExecuteAsync(sessionId, cancellationToken));

    /// <summary>
    /// Records "Know" or "Don't know" for one card. Grading the last card in the queue finishes
    /// the lesson, and the response then carries the XP, the new streak and the daily goal state.
    /// </summary>
    [HttpPost("{sessionId:guid}/grades")]
    [ProducesResponseType<StudySessionStateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudySessionStateDto>> Grade(
        Guid sessionId,
        GradeCardCommand command,
        CancellationToken cancellationToken) =>
        Respond(await gradeCard.ExecuteAsync(sessionId, command, cancellationToken));

    /// <summary>Finishes a lesson early, keeping the reward for the cards already answered.</summary>
    [HttpPost("{sessionId:guid}/complete")]
    [ProducesResponseType<StudySessionStateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudySessionStateDto>> Complete(
        Guid sessionId,
        CancellationToken cancellationToken) =>
        Respond(await completeSession.ExecuteAsync(sessionId, cancellationToken));

    /// <summary>Drops an unfinished lesson without a reward.</summary>
    [HttpDelete("{sessionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Abandon(Guid sessionId, CancellationToken cancellationToken) =>
        RespondNoContent(await abandonSession.ExecuteAsync(sessionId, cancellationToken));
}
