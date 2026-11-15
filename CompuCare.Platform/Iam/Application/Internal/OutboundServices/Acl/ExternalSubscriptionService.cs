using CompuCare.Platform.Subscriptions.Interfaces.Acl;

namespace CompuCare.Platform.Iam.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the IAM bounded context to interact with Subscriptions capabilities.
/// </summary>
/// <param name="subscriptionsContextFacade">Subscriptions bounded-context facade.</param>
public class ExternalSubscriptionService(ISubscriptionsContextFacade subscriptionsContextFacade)
{
    public async Task<bool> ExistsPlanById(int planId, CancellationToken cancellationToken)
    {
        return await subscriptionsContextFacade.ExistsPlanById(planId, cancellationToken);
    }

    /// <returns>The id of the activated subscription, or null when it could not be activated.</returns>
    public async Task<int?> ActivateSubscription(int companyId, int planId, CancellationToken cancellationToken)
    {
        var subscriptionId = await subscriptionsContextFacade.ActivateSubscription(companyId, planId, cancellationToken);
        return subscriptionId == 0 ? null : subscriptionId;
    }
}
