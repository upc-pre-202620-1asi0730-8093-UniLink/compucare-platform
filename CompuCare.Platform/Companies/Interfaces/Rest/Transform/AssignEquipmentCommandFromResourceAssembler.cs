using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class AssignEquipmentCommandFromResourceAssembler
{
    public static AssignEquipmentCommand ToCommandFromResource(int equipmentId, int companyId,
        AssignEquipmentResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new AssignEquipmentCommand(equipmentId, companyId, resource.EmployeeId);
    }
}
