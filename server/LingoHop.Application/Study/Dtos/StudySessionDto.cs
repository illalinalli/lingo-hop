using LingoHop.Application.Users.Dtos;

namespace LingoHop.Application.Study.Dtos;

/// <summary>A card as presented during a lesson, with the grade given so far.</summary>
public sealed record StudyCardDto
{
    public required Guid CardId { get; init; }

    /// <summary>Zero-based position in the session queue.</summary>
    public required int Position { get; init; }

    public required string Term { get; init; }

    public required string Translation { get; init; }

    public required string PartOfSpeech { get; init; }

    public string? Example { get; init; }

    /// <summary><c>null</c> while unanswered, otherwise the grade the learner gave.</summary>
    public bool? Known { get; init; }
}

/// <summary>A lesson in progress or just finished.</summary>
public sealed record StudySessionDto
{
    public required Guid Id { get; init; }

    public required Guid DeckId { get; init; }

    public required string DeckTitle { get; init; }

    public required string DeckIcon { get; init; }

    /// <summary><c>InProgress</c>, <c>Completed</c> or <c>Abandoned</c>.</summary>
    public required string Status { get; init; }

    public required int TotalCards { get; init; }

    public required int AnsweredCards { get; init; }

    public required int KnownCards { get; init; }

    public required int UnknownCards { get; init; }

    /// <summary>Answered share of the queue, 0..1 - the top progress bar.</summary>
    public required double Progress { get; init; }

    /// <summary>Card to show next; <c>null</c> when the queue is finished.</summary>
    public Guid? CurrentCardId { get; init; }

    /// <summary>XP granted on completion; 0 while the lesson is running.</summary>
    public required int ExperienceEarned { get; init; }

    public required DateTimeOffset StartedAtUtc { get; init; }

    public DateTimeOffset? CompletedAtUtc { get; init; }

    public required IReadOnlyList<StudyCardDto> Cards { get; init; }
}

/// <summary>
/// Response shape for every study endpoint: the lesson plus the learner's freshly
/// recalculated stats, so the client never has to re-fetch the profile.
/// </summary>
public sealed record StudySessionStateDto
{
    public required StudySessionDto Session { get; init; }

    public required LearnerProfileDto Learner { get; init; }
}
