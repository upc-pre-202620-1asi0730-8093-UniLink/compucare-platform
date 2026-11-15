using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Subscriptions.Interfaces.Acl;

namespace CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Companies bounded context to read preventive maintenance visits.
/// </summary>
/// <param name="subscriptionsContextFacade">Subscriptions bounded-context facade.</param>
public class ExternalMaintenanceService(ISubscriptionsContextFacade subscriptionsContextFacade)
{
    public async Task<IReadOnlyList<EquipmentHistoryMaintenance>> FetchMaintenancesByEquipmentId(int equipmentId,
        int companyId, CancellationToken cancellationToken)
    {
        var visits = await subscriptionsContextFacade.FetchMaintenanceVisitSummariesByEquipmentId(equipmentId,
            companyId, cancellationToken);
        return visits.Select(v => new EquipmentHistoryMaintenance(v.Id, v.Date, v.Time, v.Status)).ToList();
    }
}
