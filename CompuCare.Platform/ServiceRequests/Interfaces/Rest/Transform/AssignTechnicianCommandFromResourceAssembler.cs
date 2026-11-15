using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;

public static class AssignTechnicianCommandFromResourceAssembler
{
    public static AssignTechnicianCommand ToCommandFromResource(int serviceRequestId,
        AssignTechnicianResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new AssignTechnicianCommand(serviceRequestId, resource.TechnicianId);
    }
}
