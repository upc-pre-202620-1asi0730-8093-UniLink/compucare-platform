namespace CompuCare.Platform.Companies.Domain.Model.Entities;

/// <summary>
///     Append-only record of an ownership change of a piece of equipment.
/// </summary>
public class EquipmentAssignment
{
    public EquipmentAssignment()
    {
    }

    public EquipmentAssignment(int? employeeId)
    {
        EmployeeId = employeeId;
        AssignedAt = DateTime.UtcNow;
    }

    public int Id { get; }

    /// <summary>
    ///     The employee who received the equipment, or null when it was unassigned.
    /// </summary>
    public int? EmployeeId { get; private set; }

    public DateTime AssignedAt { get; private set; }
}
