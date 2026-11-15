using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Quotations.Interfaces.Acl;
using CompuCare.Platform.ServiceRequests.Interfaces.Acl;

namespace CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Companies bounded context to read service requests and their quotations.
/// </summary>
public class ExternalServiceRequestService(
    IServiceRequestsContextFacade serviceRequestsContextFacade,
    IQuotationsContextFacade quotationsContextFacade)
{
    public async Task<ISet<int>> FetchEquipmentIdsInRepair(int companyId, CancellationToken cancellationToken)
    {
        var equipmentIds = await serviceRequestsContextFacade.FetchEquipmentIdsWithOpenServiceRequests(companyId,
            cancellationToken);
        return equipmentIds.ToHashSet();
    }

    public async Task<IReadOnlyList<EquipmentHistoryServiceRequest>> FetchServiceRequestsByEquipmentId(
        int equipmentId, int companyId, CancellationToken cancellationToken)
    {
        var summaries = await serviceRequestsContextFacade.FetchServiceRequestSummariesByEquipmentId(equipmentId,
            companyId, cancellationToken);
        var result = new List<EquipmentHistoryServiceRequest>();
        foreach (var summary in summaries)
        {
            var quotations = await quotationsContextFacade.FetchQuotationSummariesByServiceRequestId(summary.Id,
                cancellationToken);
            result.Add(new EquipmentHistoryServiceRequest(summary.Id, summary.Category, summary.Description,
                summary.Status,
                quotations.Select(q => new EquipmentHistoryQuotation(q.Id, q.ServiceRequestId, q.Total, q.Status))
                    .ToList()));
        }

        return result;
    }
}
