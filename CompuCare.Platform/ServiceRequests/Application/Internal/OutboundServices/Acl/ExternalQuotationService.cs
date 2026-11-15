using CompuCare.Platform.Quotations.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Service Requests bounded context to interact with Quotations capabilities.
/// </summary>
/// <param name="quotationsContextFacade">Quotations bounded-context facade.</param>
public class ExternalQuotationService(IQuotationsContextFacade quotationsContextFacade)
{
    public async Task<bool> HasUnresolvedQuotations(int serviceRequestId, CancellationToken cancellationToken)
    {
        return await quotationsContextFacade.HasUnresolvedQuotations(serviceRequestId, cancellationToken);
    }
}
