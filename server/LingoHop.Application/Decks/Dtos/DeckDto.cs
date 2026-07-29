namespace LingoHop.Application.Decks.Dtos;

/// <summary>A deck tile on the home screen.</summary>
public record DeckDto
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    /// <summary>Emoji badge shown on the tile.</summary>
    public required string Icon { get; init; }

    public required int CardCount { get; init; }

    public required int LearnedCardCount { get; init; }

    /// <summary>Share of learned cards, 0..1 - the green progress bar.</summary>
    public required double Completion { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}

/// <summary>A deck together with all of its cards.</summary>
public sealed record DeckDetailsDto : DeckDto
{
    public required IReadOnlyList<CardDto> Cards { get; init; }
}
