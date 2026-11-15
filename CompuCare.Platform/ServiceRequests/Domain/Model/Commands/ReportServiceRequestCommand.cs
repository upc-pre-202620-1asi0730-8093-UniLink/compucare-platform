namespace CompuCare.Platform.ServiceRequests.Domain.Model.Commands;

/// <summary>
///     An employee reports an issue with one of the devices assigned to them.
/// </summary>
public record ReportServiceRequestCommand(int CompanyId, int RequesterId, int EquipmentId, string Category,
    string Description);
