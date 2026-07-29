using LingoHop.Domain.Common;

namespace LingoHop.Domain.Study;

/// <summary>
/// One slot in a session queue: which card, in which position, and how it was graded.
/// Entity inside the <see cref="StudySession"/> aggregate.
/// </summary>
public sealed class SessionCard : Entity
{
    internal SessionCard(Guid id, Guid sessionId, Guid cardId, int position)
        : base(id)
    {
        DomainException.Require(position >= 0, "Session card position cannot be negative.");
        SessionId = sessionId;
        CardId = cardId;
        Position = position;
    }

    private SessionCard()
    {
        // EF Core materialisation.
    }

    public Guid SessionId { get; private set; }

    /// <summary>Identifier of the <c>Card</c> in the Deck aggregate.</summary>
    public Guid CardId { get; private set; }

    /// <summary>Zero-based position in the queue.</summary>
    public int Position { get; private set; }

    /// <summary><c>null</c> until graded, then <c>true</c> for "Know" and <c>false</c> for "Don't know".</summary>
    public bool? Known { get; private set; }

    public DateTimeOffset? AnsweredAtUtc { get; private set; }

    public bool IsAnswered => Known is not null;

    internal void Grade(bool known, DateTimeOffset answeredAtUtc)
    {
        Known = known;
        AnsweredAtUtc = answeredAtUtc;
    }
}
