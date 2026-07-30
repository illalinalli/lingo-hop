using LingoHop.Domain.Common;
using LingoHop.Domain.Decks.Events;

namespace LingoHop.Domain.Decks;

/// <summary>
/// Aggregate root for a set of flashcards owned by one learner.
/// The deck is the consistency boundary for its cards: uniqueness of terms and the
/// per-card mastery counters can only be changed through this root.
/// </summary>
public sealed class Deck : AggregateRoot
{
    /// <summary>Upper bound that keeps a single aggregate small enough to load in one go.</summary>
    public const int MaxCards = 500;

    private readonly List<Card> _cards = [];

    private Deck(Guid id, Guid ownerId, DeckTitle title, DeckIcon icon, DateTimeOffset createdAtUtc)
        : base(id)
    {
        DomainException.Require(ownerId != Guid.Empty, "A deck must belong to a user.");
        OwnerId = ownerId;
        Title = title;
        Icon = icon;
        CreatedAtUtc = createdAtUtc;
    }

    private Deck()
    {
        // EF Core materialisation.
        Title = null!;
        Icon = null!;
    }

    public Guid OwnerId { get; private set; }

    public DeckTitle Title { get; private set; }

    public DeckIcon Icon { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyList<Card> Cards => _cards;

    public int CardCount => _cards.Count;

    public int LearnedCardCount => _cards.Count(card => card.Mastery.IsLearned);

    /// <summary>Cards the learner knew the last time they came up.</summary>
    public int KnownCardCount => _cards.Count(card => card.Mastery.IsKnown);

    /// <summary>
    /// Share of the deck the learner currently knows, 0..1 - drives the green progress bar on
    /// the deck tile. Straight proportion of the cards in the deck: one card known out of two
    /// reads 50%, and missing a card takes it back out of the count.
    /// </summary>
    public double Completion => _cards.Count == 0 ? 0d : (double)KnownCardCount / _cards.Count;

    public static Deck Create(Guid ownerId, DeckTitle title, DeckIcon icon, DateTimeOffset nowUtc) =>
        new(Guid.CreateVersion7(), ownerId, title, icon, nowUtc);

    public bool IsOwnedBy(Guid userId) => OwnerId == userId;

    public void Rename(DeckTitle title) => Title = title;

    public void ChangeIcon(DeckIcon icon) => Icon = icon;

    public Card AddCard(
        Term term,
        Translation translation,
        PartOfSpeech partOfSpeech,
        ExampleSentence? example,
        DateTimeOffset nowUtc)
    {
        DomainException.Require(_cards.Count < MaxCards, $"A deck cannot hold more than {MaxCards} cards.");
        DomainException.Require(
            !_cards.Any(card => card.Term.Matches(term)),
            $"The word \"{term}\" is already in this deck.");

        var card = new Card(Guid.CreateVersion7(), Id, term, translation, partOfSpeech, example, nowUtc);
        _cards.Add(card);
        return card;
    }

    public void UpdateCard(
        Guid cardId,
        Term term,
        Translation translation,
        PartOfSpeech partOfSpeech,
        ExampleSentence? example)
    {
        var card = RequireCard(cardId);
        DomainException.Require(
            !_cards.Any(other => other.Id != cardId && other.Term.Matches(term)),
            $"The word \"{term}\" is already in this deck.");

        card.Update(term, translation, partOfSpeech, example);
    }

    public void RemoveCard(Guid cardId, DateTimeOffset nowUtc)
    {
        _cards.Remove(RequireCard(cardId));

        // An unfinished lesson may still be queueing this card; it has to hear about it.
        Raise(new CardRemovedFromDeckDomainEvent(Id, OwnerId, cardId, nowUtc));
    }

    /// <summary>Applies a graded answer to the card's mastery counters.</summary>
    public void RegisterReview(Guid cardId, bool known, DateTimeOffset reviewedAtUtc) =>
        RequireCard(cardId).RegisterReview(known, reviewedAtUtc);

    /// <summary>Clears all mastery counters so the deck can be learned from scratch.</summary>
    public void ResetProgress()
    {
        foreach (var card in _cards)
        {
            card.ResetMastery();
        }
    }

    /// <summary>
    /// Picks the cards for the next session. The ordering policy belongs to the domain
    /// (unseen cards first, then the shakiest ones, learned cards last), while the
    /// randomness inside each group is supplied by the caller so the domain stays testable.
    /// </summary>
    public IReadOnlyList<Card> SelectCardsForStudy(
        int limit,
        Func<IReadOnlyList<Card>, IReadOnlyList<Card>> shuffle)
    {
        DomainException.Require(limit > 0, "A study session must contain at least one card.");
        DomainException.Require(_cards.Count > 0, "Add at least one card before studying this deck.");

        // OrderBy is a stable sort, so the shuffled order survives inside each priority group.
        return [.. shuffle(_cards)
            .OrderBy(card => card.Mastery.SessionPriority)
            .ThenBy(card => card.Mastery.Accuracy)
            .Take(limit)];
    }

    public Card? FindCard(Guid cardId) => _cards.FirstOrDefault(card => card.Id == cardId);

    private Card RequireCard(Guid cardId) =>
        FindCard(cardId) ?? throw new DomainException("This card is not part of the deck.");
}
