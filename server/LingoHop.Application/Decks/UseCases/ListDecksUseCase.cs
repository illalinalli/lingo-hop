using LingoHop.Application.Abstractions;
using LingoHop.Application.Common;
using LingoHop.Application.Decks.Dtos;
using LingoHop.Domain.Decks;

namespace LingoHop.Application.Decks.UseCases;

/// <summary>All decks owned by the current learner, newest first.</summary>
public sealed class ListDecksUseCase(ICurrentLearner currentLearner, IDeckRepository decks)
{
    public async Task<Result<IReadOnlyList<DeckDto>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var user = await currentLearner.GetAsync(cancellationToken);
        var owned = await decks.ListByOwnerAsync(user.Id, cancellationToken);

        return Result.Success<IReadOnlyList<DeckDto>>([.. owned.Select(deck => deck.ToDto())]);
    }
}
