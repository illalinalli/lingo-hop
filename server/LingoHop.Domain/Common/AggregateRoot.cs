namespace LingoHop.Domain.Common;

/// <summary>
/// Consistency boundary. Only aggregate roots are loaded and saved through repositories;
/// everything else is reached through the root.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    /// <summary>Constructor used by EF Core materialisation only.</summary>
    protected AggregateRoot()
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
