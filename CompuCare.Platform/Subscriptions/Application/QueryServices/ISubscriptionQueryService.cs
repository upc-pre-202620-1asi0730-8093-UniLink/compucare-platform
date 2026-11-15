using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;

namespace CompuCare.Platform.Subscriptions.Application.QueryServices;

/// <summary>
///     The subscription query service.
/// </summary>
public interface ISubscriptionQueryService
{
    Task<Subscription?> Handle(GetSubscriptionByCompanyIdQuery query, CancellationToken cancellationToken);
}
