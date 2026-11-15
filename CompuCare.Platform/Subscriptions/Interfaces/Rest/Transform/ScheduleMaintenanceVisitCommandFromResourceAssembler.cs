using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class ScheduleMaintenanceVisitCommandFromResourceAssembler
{
    public static ScheduleMaintenanceVisitCommand ToCommandFromResource(ScheduleMaintenanceResource resource,
        int companyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ScheduleMaintenanceVisitCommand(companyId, resource.Date, resource.Time,
            resource.EquipmentIds ?? []);
    }
}
