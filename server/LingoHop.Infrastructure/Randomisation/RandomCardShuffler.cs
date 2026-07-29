using LingoHop.Application.Abstractions;

namespace LingoHop.Infrastructure.Randomisation;

/// <summary>Fisher-Yates shuffle over <see cref="Random.Shared"/>.</summary>
internal sealed class RandomCardShuffler : ICardShuffler
{
    public IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items)
    {
        var shuffled = items.ToArray();
        Random.Shared.Shuffle(shuffled);
        return shuffled;
    }
}
