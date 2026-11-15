using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class MaintenanceResourceFromEntityAssembler
{
    public static MaintenanceResource ToResourceFromEntity(MaintenanceVisit visit)
    {
        ArgumentNullException.ThrowIfNull(visit);
        return new MaintenanceResource(visit.Id, visit.CompanyId, visit.Date, visit.Time, visit.EquipmentIds.ToList(),
            visit.Status);
    }
}
