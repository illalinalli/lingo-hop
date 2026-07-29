using LingoHop.Domain.Common;

namespace LingoHop.Application.Abstractions.Events;

/// <summary>
/// Routes domain events to their handlers. The EF Core unit of work calls this just before
/// committing, so handler side effects join the same transaction.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
