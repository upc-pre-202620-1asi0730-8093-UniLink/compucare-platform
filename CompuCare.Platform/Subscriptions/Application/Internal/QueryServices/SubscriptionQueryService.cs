using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;
using CompuCare.Platform.Subscriptions.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    /// <inheritdoc />
    public async Task<Subscription?> Handle(GetSubscriptionByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await subscriptionRepository.FindByCompanyIdAsNoTrackingAsync(query.CompanyId, cancellationToken);
    }
}
