using CompuCare.Platform.Shared.Domain.Model.Aggregates;
using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

/// <summary>
///     EF Core interceptor that automatically dispatches domain events recorded on aggregate roots
///     once changes have been saved to the database.
/// </summary>
public sealed class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    ///     Runs from <c>SavedChanges</c>, after the insert/update has actually completed — not from
    ///     <c>SavingChanges</c>. <see cref="AggregateRoot.DomainEvents" /> builds each event lazily from a
    ///     factory, precisely so that reading it here, now, has the aggregate's real database-generated id
    ///     to close over; reading it any earlier (including just storing a reference to read later) would
    ///     still invoke those factories before the id exists, baking in <c>0</c> regardless of when the
    ///     resulting event objects are actually published.
    /// </summary>
    private async Task DispatchDomainEvents(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return;

        var aggregateRoots = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            // Cortex.Mediator's PublishAsync<TNotification> is generic, with TNotification inferred at the
            // call site. Every event here is boxed as the IEvent interface (DomainEvents is
            // IReadOnlyCollection<IEvent>), so without `dynamic`, TNotification would be inferred as IEvent
            // itself — a type nothing implements IEventHandler<IEvent> for — and the mediator would silently
            // find zero handlers. Casting to dynamic forces the generic argument to resolve against the
            // event's real runtime type instead.
            await mediator.PublishAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}
