using LingoHop.Domain.Common;

namespace LingoHop.Domain.Decks;

/// <summary>
/// A flashcard. An entity inside the <see cref="Deck"/> aggregate: it is never loaded or
/// mutated on its own, which is why the constructor and mutators are <c>internal</c> -
/// all changes go through the deck so invariants (unique terms) stay enforceable.
/// </summary>
public sealed class Card : Entity
{
    internal Card(
        Guid id,
        Guid deckId,
        Term term,
        Translation translation,
        PartOfSpeech partOfSpeech,
        ExampleSentence? example,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        DeckId = deckId;
        Term = term;
        Translation = translation;
        PartOfSpeech = partOfSpeech;
        Example = example;
        Mastery = CardMastery.New();
        CreatedAtUtc = createdAtUtc;
    }

    private Card()
    {
        // EF Core materialisation.
        Term = null!;
        Translation = null!;
        Mastery = null!;
    }

    public Guid DeckId { get; private set; }

    public Term Term { get; private set; }

    public Translation Translation { get; private set; }

    public PartOfSpeech PartOfSpeech { get; private set; }

    public ExampleSentence? Example { get; private set; }

    public CardMastery Mastery { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal void Update(
        Term term,
        Translation translation,
        PartOfSpeech partOfSpeech,
        ExampleSentence? example)
    {
        Term = term;
        Translation = translation;
        PartOfSpeech = partOfSpeech;
        Example = example;
    }

    internal void RegisterReview(bool known, DateTimeOffset reviewedAtUtc) =>
        Mastery = known ? Mastery.RegisterKnown(reviewedAtUtc) : Mastery.RegisterUnknown(reviewedAtUtc);

    internal void ResetMastery() => Mastery = CardMastery.New();
}
