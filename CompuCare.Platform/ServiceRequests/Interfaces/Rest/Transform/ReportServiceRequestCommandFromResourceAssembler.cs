using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;

public static class ReportServiceRequestCommandFromResourceAssembler
{
    public static ReportServiceRequestCommand ToCommandFromResource(ReportServiceRequestResource resource,
        int companyId, int requesterId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ReportServiceRequestCommand(companyId, requesterId, resource.EquipmentId,
            resource.Category ?? string.Empty, resource.Description ?? string.Empty);
    }
}
