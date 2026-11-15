using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class CreateEquipmentCommandFromResourceAssembler
{
    public static CreateEquipmentCommand ToCommandFromResource(CreateEquipmentResource resource, int companyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateEquipmentCommand(companyId, resource.Name, resource.SerialNumber, resource.Type,
            resource.LocationId);
    }
}
