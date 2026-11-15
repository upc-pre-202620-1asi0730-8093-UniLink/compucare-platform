namespace CompuCare.Platform.Subscriptions.Domain.Model.Commands;

/// <summary>
///     Books a preventive maintenance visit for some devices of a company.
/// </summary>
public record ScheduleMaintenanceVisitCommand(int CompanyId, string Date, string Time, IReadOnlyList<int> EquipmentIds);
