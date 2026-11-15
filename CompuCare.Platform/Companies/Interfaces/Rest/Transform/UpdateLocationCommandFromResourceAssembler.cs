using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class UpdateLocationCommandFromResourceAssembler
{
    public static UpdateLocationCommand ToCommandFromResource(int locationId, int companyId,
        UpdateLocationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateLocationCommand(locationId, companyId, resource.Name, resource.Desk, resource.Active);
    }
}
