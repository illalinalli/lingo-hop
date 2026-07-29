namespace LingoHop.Domain.Decks;

/// <summary>Persistence contract for the <see cref="Deck"/> aggregate.</summary>
public interface IDeckRepository
{
    /// <summary>Loads a deck together with its cards.</summary>
    Task<Deck?> FindByIdAsync(Guid deckId, CancellationToken cancellationToken = default);

    /// <summary>All decks owned by a user, newest first, cards included.</summary>
    Task<IReadOnlyList<Deck>> ListByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<int> CountByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    void Add(Deck deck);

    void Remove(Deck deck);
}
