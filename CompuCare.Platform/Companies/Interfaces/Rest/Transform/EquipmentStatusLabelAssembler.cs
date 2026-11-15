using CompuCare.Platform.Companies.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

/// <summary>
///     Translates equipment statuses to and from the labels used by the web application.
/// </summary>
public static class EquipmentStatusLabelAssembler
{
    private const string OperationalLabel = "Operativo";
    private const string InRepairLabel = "En Reparación";

    public static string ToLabelFromStatus(EEquipmentStatus status) =>
        status == EEquipmentStatus.InRepair ? InRepairLabel : OperationalLabel;

    public static EEquipmentStatus? ToStatusFromLabel(string? label) => label switch
    {
        OperationalLabel => EEquipmentStatus.Operational,
        InRepairLabel => EEquipmentStatus.InRepair,
        _ => null
    };
}
