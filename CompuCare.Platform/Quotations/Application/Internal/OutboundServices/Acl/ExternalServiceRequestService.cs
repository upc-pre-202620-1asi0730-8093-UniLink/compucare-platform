using CompuCare.Platform.ServiceRequests.Interfaces.Acl;

namespace CompuCare.Platform.Quotations.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Quotations bounded context to interact with Service Requests capabilities.
/// </summary>
/// <param name="serviceRequestsContextFacade">Service Requests bounded-context facade.</param>
public class ExternalServiceRequestService(IServiceRequestsContextFacade serviceRequestsContextFacade)
{
    public async Task<QuotableServiceRequest?> FetchQuotableServiceRequest(int serviceRequestId, int technicianId,
        CancellationToken cancellationToken)
    {
        return await serviceRequestsContextFacade.FetchQuotableServiceRequest(serviceRequestId, technicianId,
            cancellationToken);
    }

    public async Task<bool> SetServiceRequestPaused(int serviceRequestId, int companyId, bool paused,
        CancellationToken cancellationToken)
    {
        return await serviceRequestsContextFacade.SetServiceRequestPaused(serviceRequestId, companyId, paused,
            cancellationToken);
    }
}
