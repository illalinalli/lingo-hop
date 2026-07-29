using LingoHop.Domain.Common;
using LingoHop.Domain.Study.Events;

namespace LingoHop.Domain.Study;

/// <summary>
/// Aggregate root for one pass through a deck: a fixed, ordered queue of cards plus the
/// grade the learner gave each of them. Fixing the queue up front is what makes the
/// progress bar meaningful and lets an interrupted lesson be resumed.
/// </summary>
public sealed class StudySession : AggregateRoot
{
    private readonly List<SessionCard> _cards = [];

    private StudySession(Guid id, Guid userId, Guid deckId, DateTimeOffset startedAtUtc)
        : base(id)
    {
        DomainException.Require(userId != Guid.Empty, "A session must belong to a user.");
        DomainException.Require(deckId != Guid.Empty, "A session must belong to a deck.");
        UserId = userId;
        DeckId = deckId;
        Status = StudySessionStatus.InProgress;
        StartedAtUtc = startedAtUtc;
    }

    private StudySession()
    {
        // EF Core materialisation.
    }

    public Guid UserId { get; private set; }

    public Guid DeckId { get; private set; }

    public StudySessionStatus Status { get; private set; }

    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    /// <summary>XP granted at completion; 0 while the session is running.</summary>
    public int ExperienceEarned { get; private set; }

    /// <summary>The queued cards. Backed directly by the aggregate's collection.</summary>
    public IReadOnlyList<SessionCard> Cards => _cards;

    /// <summary>The queue in presentation order.</summary>
    public IEnumerable<SessionCard> Queue => _cards.OrderBy(card => card.Position);

    public int TotalCards => _cards.Count;

    public int AnsweredCards => _cards.Count(card => card.IsAnswered);

    public int KnownCards => _cards.Count(card => card.Known == true);

    public int UnknownCards => _cards.Count(card => card.Known == false);

    public bool IsInProgress => Status == StudySessionStatus.InProgress;

    public bool IsFullyAnswered => _cards.Count > 0 && _cards.All(card => card.IsAnswered);

    /// <summary>Fraction of the queue already graded, 0..1 - drives the top progress bar.</summary>
    public double Progress => _cards.Count == 0 ? 0d : (double)AnsweredCards / _cards.Count;

    /// <summary>Next card to show, or <c>null</c> when the queue is exhausted.</summary>
    public SessionCard? CurrentCard =>
        _cards.Where(card => !card.IsAnswered).OrderBy(card => card.Position).FirstOrDefault();

    public static StudySession Start(
        Guid userId,
        Guid deckId,
        IReadOnlyList<Guid> orderedCardIds,
        DateTimeOffset nowUtc)
    {
        DomainException.Require(orderedCardIds.Count > 0, "A study session needs at least one card.");
        DomainException.Require(
            orderedCardIds.Distinct().Count() == orderedCardIds.Count,
            "A card cannot appear twice in the same session.");

        var session = new StudySession(Guid.CreateVersion7(), userId, deckId, nowUtc);
        for (var position = 0; position < orderedCardIds.Count; position++)
        {
            session._cards.Add(new SessionCard(
                Guid.CreateVersion7(),
                session.Id,
                orderedCardIds[position],
                position));
        }

        return session;
    }

    /// <summary>
    /// Grades a card ("Know" / "Don't know"). Completes the session automatically once the
    /// last card in the queue has been answered.
    /// </summary>
    public void Grade(Guid cardId, bool known, DateTimeOffset nowUtc)
    {
        DomainException.Require(IsInProgress, "This lesson has already been finished.");

        var sessionCard = _cards.FirstOrDefault(card => card.CardId == cardId)
                          ?? throw new DomainException("This card is not part of the lesson.");
        DomainException.Require(!sessionCard.IsAnswered, "This card has already been graded.");

        sessionCard.Grade(known, nowUtc);
        Raise(new CardReviewedDomainEvent(Id, DeckId, cardId, known, nowUtc));

        if (IsFullyAnswered)
        {
            Complete(nowUtc);
        }
    }

    /// <summary>Finishes the lesson early, keeping the reward for what was answered.</summary>
    public void Complete(DateTimeOffset nowUtc)
    {
        DomainException.Require(IsInProgress, "This lesson has already been finished.");
        DomainException.Require(AnsweredCards > 0, "Grade at least one card before finishing the lesson.");

        Status = StudySessionStatus.Completed;
        CompletedAtUtc = nowUtc;
        ExperienceEarned = StudyRewardPolicy.CalculateExperience(AnsweredCards, KnownCards);

        Raise(new StudySessionCompletedDomainEvent(
            Id,
            UserId,
            DeckId,
            TotalCards,
            AnsweredCards,
            KnownCards,
            ExperienceEarned,
            nowUtc));
    }

    /// <summary>Drops the lesson without a reward - used when the learner walks away.</summary>
    public void Abandon(DateTimeOffset nowUtc)
    {
        DomainException.Require(IsInProgress, "This lesson has already been finished.");
        Status = StudySessionStatus.Abandoned;
        CompletedAtUtc = nowUtc;
    }

    public bool IsOwnedBy(Guid userId) => UserId == userId;
}
