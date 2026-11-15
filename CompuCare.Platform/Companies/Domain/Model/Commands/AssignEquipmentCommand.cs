namespace CompuCare.Platform.Companies.Domain.Model.Commands;

/// <summary>
///     Assigns a piece of equipment to an employee, or unassigns it when <paramref name="EmployeeId" /> is null.
/// </summary>
public record AssignEquipmentCommand(int EquipmentId, int CompanyId, int? EmployeeId);
