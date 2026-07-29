using LingoHop.Application.Abstractions;
using LingoHop.Application.Abstractions.Events;
using LingoHop.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LingoHop.Infrastructure.Persistence;

/// <summary>
/// Commits a use case. Domain events collected on the tracked aggregates are dispatched
/// <em>before</em> <c>SaveChanges</c>, so whatever the handlers change is written in the
/// same transaction as the change that raised the event.
/// </summary>
internal sealed class UnitOfWork(LingoHopDbContext context, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    /// <summary>Guards against a handler chain that keeps raising new events forever.</summary>
    private const int MaxDispatchPasses = 8;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await context.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        for (var pass = 0; pass < MaxDispatchPasses; pass++)
        {
            var roots = context.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(entry => entry.Entity)
                .Where(root => root.DomainEvents.Count > 0)
                .ToList();

            if (roots.Count == 0)
            {
                return;
            }

            var domainEvents = roots.SelectMany(root => root.DomainEvents).ToList();

            // Cleared up front so a handler raising further events cannot cause a re-dispatch
            // of the events currently being handled.
            foreach (var root in roots)
            {
                root.ClearDomainEvents();
            }

            await dispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        throw new InvalidOperationException(
            $"Domain event dispatch did not settle after {MaxDispatchPasses} passes - check for a handler cycle.");
    }
}
