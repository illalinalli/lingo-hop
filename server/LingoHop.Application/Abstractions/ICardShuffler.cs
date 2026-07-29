namespace LingoHop.Application.Abstractions;

/// <summary>
/// Source of randomness for building a session queue. Injected so the Domain layer can
/// own the ordering policy while staying deterministic under test.
/// </summary>
public interface ICardShuffler
{
    IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items);
}
