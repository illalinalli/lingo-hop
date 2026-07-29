using LingoHop.Domain.Decks;
using Microsoft.EntityFrameworkCore;

namespace LingoHop.Infrastructure.Persistence.Repositories;

internal sealed class DeckRepository(LingoHopDbContext context) : IDeckRepository
{
    /// <remarks>Cards come along automatically - the navigation is configured with AutoInclude.</remarks>
    public Task<Deck?> FindByIdAsync(Guid deckId, CancellationToken cancellationToken = default) =>
        context.Decks.FirstOrDefaultAsync(deck => deck.Id == deckId, cancellationToken);

    public async Task<IReadOnlyList<Deck>> ListByOwnerAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default) =>
        await context.Decks
            .Where(deck => deck.OwnerId == ownerId)
            .OrderByDescending(deck => deck.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<int> CountByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default) =>
        context.Decks.CountAsync(deck => deck.OwnerId == ownerId, cancellationToken);

    public void Add(Deck deck) => context.Decks.Add(deck);

    public void Remove(Deck deck) => context.Decks.Remove(deck);
}
