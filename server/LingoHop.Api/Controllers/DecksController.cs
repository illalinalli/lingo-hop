using LingoHop.Application.Decks.Dtos;
using LingoHop.Application.Decks.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.Controllers;

/// <summary>Decks owned by the current learner.</summary>
[Route("api/decks")]
[Tags("Decks")]
public sealed class DecksController(
    ListDecksUseCase listDecks,
    GetDeckUseCase getDeck,
    CreateDeckUseCase createDeck,
    UpdateDeckUseCase updateDeck,
    DeleteDeckUseCase deleteDeck,
    ResetDeckProgressUseCase resetDeckProgress) : ApiControllerBase
{
    /// <summary>All decks of the current learner, newest first.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<DeckDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DeckDto>>> List(CancellationToken cancellationToken) =>
        Respond(await listDecks.ExecuteAsync(cancellationToken));

    /// <summary>A single deck with all of its cards.</summary>
    [HttpGet("{deckId:guid}")]
    [ProducesResponseType<DeckDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeckDetailsDto>> Get(Guid deckId, CancellationToken cancellationToken) =>
        Respond(await getDeck.ExecuteAsync(deckId, cancellationToken));

    /// <summary>Creates a deck, optionally with its first cards in the same call.</summary>
    [HttpPost]
    [ProducesResponseType<DeckDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DeckDetailsDto>> Create(
        CreateDeckCommand command,
        CancellationToken cancellationToken)
    {
        var result = await createDeck.ExecuteAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return Failure(result.Error);
        }

        return CreatedAtAction(nameof(Get), new { deckId = result.Value.Id }, result.Value);
    }

    /// <summary>Renames a deck and/or changes its emoji.</summary>
    [HttpPut("{deckId:guid}")]
    [ProducesResponseType<DeckDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeckDetailsDto>> Update(
        Guid deckId,
        UpdateDeckCommand command,
        CancellationToken cancellationToken) =>
        Respond(await updateDeck.ExecuteAsync(deckId, command, cancellationToken));

    /// <summary>Deletes a deck with its cards and study history.</summary>
    [HttpDelete("{deckId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid deckId, CancellationToken cancellationToken) =>
        RespondNoContent(await deleteDeck.ExecuteAsync(deckId, cancellationToken));

    /// <summary>Clears every card's mastery counters so the deck can be learned again.</summary>
    [HttpPost("{deckId:guid}/reset-progress")]
    [ProducesResponseType<DeckDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeckDetailsDto>> ResetProgress(
        Guid deckId,
        CancellationToken cancellationToken) =>
        Respond(await resetDeckProgress.ExecuteAsync(deckId, cancellationToken));
}
