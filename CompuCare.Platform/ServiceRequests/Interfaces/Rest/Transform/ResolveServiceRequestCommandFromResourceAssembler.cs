using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;

public static class ResolveServiceRequestCommandFromResourceAssembler
{
    public static ResolveServiceRequestCommand ToCommandFromResource(int serviceRequestId, int technicianId,
        ResolveServiceRequestResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ResolveServiceRequestCommand(serviceRequestId, technicianId, resource.Diagnosis ?? string.Empty,
            resource.WorkDone ?? string.Empty, resource.Hours);
    }
}
