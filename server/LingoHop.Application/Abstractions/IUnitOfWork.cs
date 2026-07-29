namespace LingoHop.Application.Abstractions;

/// <summary>
/// Transaction boundary for a use case. Implemented by the EF Core adapter, which also
/// dispatches the domain events collected on the tracked aggregates before committing.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
