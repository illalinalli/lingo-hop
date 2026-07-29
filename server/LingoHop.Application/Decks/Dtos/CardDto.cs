namespace LingoHop.Application.Decks.Dtos;

/// <summary>A flashcard together with its mastery counters.</summary>
public sealed record CardDto
{
    public required Guid Id { get; init; }

    /// <summary>Front of the card - the word being learned.</summary>
    public required string Term { get; init; }

    /// <summary>Back of the card - the meaning.</summary>
    public required string Translation { get; init; }

    /// <summary>Grammar tag, e.g. <c>Noun</c>; <c>Unspecified</c> when not set.</summary>
    public required string PartOfSpeech { get; init; }

    public string? Example { get; init; }

    public required int TimesSeen { get; init; }

    public required int TimesKnown { get; init; }

    public required int CorrectStreak { get; init; }

    public required bool IsLearned { get; init; }

    /// <summary>Share of correct answers, 0..1.</summary>
    public required double Accuracy { get; init; }

    public DateTimeOffset? LastReviewedAtUtc { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}
