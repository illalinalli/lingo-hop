using LingoHop.Application.Decks.Dtos;
using LingoHop.Application.Decks.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LingoHop.Api.Controllers;

/// <summary>Cards inside a deck. Nested under the deck because a card never exists on its own.</summary>
[Route("api/decks/{deckId:guid}/cards")]
[Tags("Cards")]
public sealed class CardsController(
    AddCardUseCase addCard,
    UpdateCardUseCase updateCard,
    DeleteCardUseCase deleteCard) : ApiControllerBase
{
    /// <summary>Adds a card. The word must not already exist in the deck.</summary>
    [HttpPost]
    [ProducesResponseType<CardDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> Add(
        Guid deckId,
        CardDraft draft,
        CancellationToken cancellationToken)
    {
        var result = await addCard.ExecuteAsync(deckId, draft, cancellationToken);
        if (!result.IsSuccess)
        {
            return Failure(result.Error);
        }

        return Created($"/api/decks/{deckId}/cards/{result.Value.Id}", result.Value);
    }

    /// <summary>Edits a card's text. Mastery counters are kept.</summary>
    [HttpPut("{cardId:guid}")]
    [ProducesResponseType<CardDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> Update(
        Guid deckId,
        Guid cardId,
        CardDraft draft,
        CancellationToken cancellationToken) =>
        Respond(await updateCard.ExecuteAsync(deckId, cardId, draft, cancellationToken));

    /// <summary>Removes a card from the deck.</summary>
    [HttpDelete("{cardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        Guid deckId,
        Guid cardId,
        CancellationToken cancellationToken) =>
        RespondNoContent(await deleteCard.ExecuteAsync(deckId, cardId, cancellationToken));
}
