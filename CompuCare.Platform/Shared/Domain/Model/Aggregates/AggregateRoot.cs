using CompuCare.Platform.Shared.Domain.Model.Events;

namespace CompuCare.Platform.Shared.Domain.Model.Aggregates;

/// <summary>
///     Base class for aggregate roots in Domain-Driven Design.
///     Encapsulates domain event collection and management.
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<Func<IEvent>> _domainEventFactories = [];

    /// <inheritdoc />
    /// <remarks>
    ///     Built lazily, one factory invocation per read, rather than stored as already-constructed event
    ///     objects. A "Created" event raised from inside an aggregate's own constructor — the only place that
    ///     can raise one — closes over an id that doesn't have its real, database-generated value yet; a factory
    ///     re-reads that id at whatever point <see cref="DomainEvents" /> is actually read (after
    ///     <c>SaveChanges</c> has run), instead of baking in the stale value a plain object would have captured
    ///     at construction time.
    /// </remarks>
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEventFactories.Select(factory => factory()).ToList().AsReadOnly();

    /// <summary>
    ///     Records a domain event to be dispatched when changes are committed.
    /// </summary>
    /// <param name="domainEventFactory">Builds the domain event, evaluated when <see cref="DomainEvents" /> is read.</param>
    protected void AddDomainEvent(Func<IEvent> domainEventFactory)
    {
        _domainEventFactories.Add(domainEventFactory);
    }

    /// <inheritdoc />
    public void ClearDomainEvents()
    {
        _domainEventFactories.Clear();
    }
}
