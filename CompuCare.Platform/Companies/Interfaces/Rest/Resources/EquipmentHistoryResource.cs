namespace CompuCare.Platform.Companies.Interfaces.Rest.Resources;

public record EquipmentAssignmentResource(int Id, int EquipmentId, int? EmployeeId, DateTime At);

public record EquipmentHistoryQuoteResource(int Id, int TicketId, decimal Total, string Status);

public record EquipmentHistoryTicketResource(
    int Id,
    string Category,
    string Description,
    string Status,
    IEnumerable<EquipmentHistoryQuoteResource> Quotes);

public record EquipmentHistoryMaintenanceResource(int Id, string Date, string Time, string Status);

public record EquipmentHistoryResource(
    EquipmentResource Equipment,
    IEnumerable<EquipmentAssignmentResource> Assignments,
    IEnumerable<EquipmentHistoryTicketResource> Tickets,
    IEnumerable<EquipmentHistoryMaintenanceResource> Maintenances);
