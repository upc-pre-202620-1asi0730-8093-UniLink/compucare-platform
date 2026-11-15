using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class EquipmentResourceFromEntityAssembler
{
    public static EquipmentResource ToResourceFromEntity(EquipmentWithStatus equipmentWithStatus)
    {
        ArgumentNullException.ThrowIfNull(equipmentWithStatus);
        var equipment = equipmentWithStatus.Equipment;
        return new EquipmentResource(equipment.Id, equipment.CompanyId, equipment.Code, equipment.Name,
            equipment.SerialNumber, equipment.Type, equipment.LocationId, equipment.AssignedEmployeeId,
            EquipmentStatusLabelAssembler.ToLabelFromStatus(equipmentWithStatus.Status));
    }
}
