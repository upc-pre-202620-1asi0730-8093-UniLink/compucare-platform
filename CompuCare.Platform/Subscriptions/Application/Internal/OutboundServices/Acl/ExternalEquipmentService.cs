using CompuCare.Platform.Companies.Interfaces.Acl;

namespace CompuCare.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Subscriptions bounded context to interact with Companies capabilities.
/// </summary>
/// <param name="companiesContextFacade">Companies bounded-context facade.</param>
public class ExternalEquipmentService(ICompaniesContextFacade companiesContextFacade)
{
    public async Task<bool> AllEquipmentsBelongToCompany(IEnumerable<int> equipmentIds, int companyId,
        CancellationToken cancellationToken)
    {
        return await companiesContextFacade.AllEquipmentsBelongToCompany(equipmentIds, companyId, cancellationToken);
    }
}
