using CompuCare.Platform.Companies.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Service Requests bounded context to interact with Companies capabilities.
/// </summary>
/// <param name="companiesContextFacade">Companies bounded-context facade.</param>
public class ExternalEquipmentService(ICompaniesContextFacade companiesContextFacade)
{
    public async Task<bool> IsEquipmentAssignedToEmployee(int equipmentId, int companyId, int employeeId,
        CancellationToken cancellationToken)
    {
        return await companiesContextFacade.IsEquipmentAssignedToEmployee(equipmentId, companyId, employeeId,
            cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, string>> FetchEquipmentNamesByIds(IEnumerable<int> equipmentIds,
        CancellationToken cancellationToken)
    {
        return await companiesContextFacade.FetchEquipmentNamesByIds(equipmentIds, cancellationToken);
    }
}
