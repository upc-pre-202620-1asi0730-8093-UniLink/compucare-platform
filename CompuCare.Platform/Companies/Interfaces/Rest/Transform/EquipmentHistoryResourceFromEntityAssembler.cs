using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class EquipmentHistoryResourceFromEntityAssembler
{
    public static EquipmentHistoryResource ToResourceFromEntity(EquipmentHistory history)
    {
        ArgumentNullException.ThrowIfNull(history);
        var equipment = history.Equipment.Equipment;
        return new EquipmentHistoryResource(
            EquipmentResourceFromEntityAssembler.ToResourceFromEntity(history.Equipment),
            equipment.Assignments
                .OrderByDescending(a => a.AssignedAt)
                .Select(a => new EquipmentAssignmentResource(a.Id, equipment.Id, a.EmployeeId, a.AssignedAt))
                .ToList(),
            history.ServiceRequests
                .Select(r => new EquipmentHistoryTicketResource(r.Id, r.Category, r.Description, r.Status,
                    r.Quotations.Select(q => new EquipmentHistoryQuoteResource(q.Id, q.ServiceRequestId, q.Total,
                        q.Status)).ToList()))
                .ToList(),
            history.Maintenances
                .Select(m => new EquipmentHistoryMaintenanceResource(m.Id, m.Date, m.Time, m.Status))
                .ToList());
    }
}
