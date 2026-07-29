using LingoHop.Application.Abstractions.Events;
using LingoHop.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LingoHop.Infrastructure.Events;

/// <summary>
/// Resolves <see cref="IDomainEventHandler{TEvent}"/> implementations from the container and
/// invokes them. Reflection is confined to this one class.
/// </summary>
internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private const string HandleMethodName = nameof(IDomainEventHandler<IDomainEvent>.HandleAsync);

    public async Task DispatchAsync(
        IReadOnlyList<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handleMethod = handlerType.GetMethod(HandleMethodName)
                               ?? throw new InvalidOperationException(
                                   $"{handlerType} does not expose {HandleMethodName}.");

            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                if (handler is null)
                {
                    continue;
                }

                await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
