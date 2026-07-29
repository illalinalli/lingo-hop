namespace LingoHop.Application.Decks.Dtos;

/// <summary>Input shape for creating or editing a card.</summary>
/// <param name="Term">Front of the card - the word being learned.</param>
/// <param name="Translation">Back of the card - the meaning.</param>
/// <param name="PartOfSpeech">Optional grammar tag, e.g. <c>noun</c>.</param>
/// <param name="Example">Optional usage example.</param>
public sealed record CardDraft(
    string Term,
    string Translation,
    string? PartOfSpeech = null,
    string? Example = null);
