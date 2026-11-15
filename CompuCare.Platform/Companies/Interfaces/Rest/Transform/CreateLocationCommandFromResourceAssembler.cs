using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class CreateLocationCommandFromResourceAssembler
{
    public static CreateLocationCommand ToCommandFromResource(CreateLocationResource resource, int companyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateLocationCommand(companyId, resource.Name, resource.Desk);
    }
}
