using LingoHop.Domain.Common;

namespace LingoHop.Application.Abstractions.Events;

/// <summary>
/// Reacts to a domain event. Handlers run inside the same unit of work as the change that
/// raised the event, so they may load and mutate other aggregates.
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
