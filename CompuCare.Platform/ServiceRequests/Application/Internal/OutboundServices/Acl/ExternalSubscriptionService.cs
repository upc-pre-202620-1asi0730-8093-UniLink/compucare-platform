using CompuCare.Platform.Subscriptions.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Service Requests bounded context to interact with Subscriptions capabilities.
/// </summary>
/// <param name="subscriptionsContextFacade">Subscriptions bounded-context facade.</param>
public class ExternalSubscriptionService(ISubscriptionsContextFacade subscriptionsContextFacade)
{
    public async Task<SupportHoursConsumption> RecordSupportHoursConsumption(int companyId, int serviceRequestId,
        decimal hours, CancellationToken cancellationToken)
    {
        return await subscriptionsContextFacade.RecordSupportHoursConsumption(companyId, serviceRequestId, hours,
            cancellationToken);
    }
}
