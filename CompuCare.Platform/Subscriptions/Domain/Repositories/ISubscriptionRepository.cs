using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Domain.Repositories;

/// <summary>
///     The subscription repository.
/// </summary>
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);

    Task<Subscription?> FindByCompanyIdAsNoTrackingAsync(int companyId, CancellationToken cancellationToken);
}
