namespace CompuCare.Platform.Subscriptions.Interfaces.Acl;

/// <summary>
///     Published language of the Subscriptions context for a preventive maintenance visit.
/// </summary>
public record MaintenanceVisitSummary(int Id, string Date, string Time, string Status);
