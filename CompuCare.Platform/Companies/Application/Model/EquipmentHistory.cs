namespace CompuCare.Platform.Companies.Application.Model;

/// <summary>
///     A quotation issued for a service request of the equipment.
/// </summary>
public record EquipmentHistoryQuotation(int Id, int ServiceRequestId, decimal Total, string Status);

/// <summary>
///     A service request reported for the equipment.
/// </summary>
public record EquipmentHistoryServiceRequest(
    int Id,
    string Category,
    string Description,
    string Status,
    IReadOnlyList<EquipmentHistoryQuotation> Quotations);

/// <summary>
///     A preventive maintenance visit that includes the equipment.
/// </summary>
public record EquipmentHistoryMaintenance(int Id, string Date, string Time, string Status);

/// <summary>
///     Everything that happened to a piece of equipment, gathered from the contexts that own each part.
/// </summary>
public record EquipmentHistory(
    EquipmentWithStatus Equipment,
    IReadOnlyList<EquipmentHistoryServiceRequest> ServiceRequests,
    IReadOnlyList<EquipmentHistoryMaintenance> Maintenances);
