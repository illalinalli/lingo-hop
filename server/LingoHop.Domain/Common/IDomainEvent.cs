namespace LingoHop.Domain.Common;

/// <summary>
/// Marker for something meaningful that happened inside an aggregate.
/// Events are dispatched by the Application layer inside the same transaction
/// as the change that raised them.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}
