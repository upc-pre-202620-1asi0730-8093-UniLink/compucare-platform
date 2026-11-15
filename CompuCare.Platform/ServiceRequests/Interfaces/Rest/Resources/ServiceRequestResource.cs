namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;

public record ServiceRequestTimelineResource(string Status, DateTime At);

public record ServiceRequestResource(
    int Id,
    int CompanyId,
    int EmployeeId,
    int EquipmentId,
    int? TechnicianId,
    string Category,
    string Description,
    string Status,
    bool Paused,
    DateTimeOffset? CreatedAt,
    string? EquipmentName,
    string? EmployeeName,
    string? TechnicianName,
    string? Diagnosis,
    string? WorkDone,
    decimal Hours,
    IEnumerable<ServiceRequestTimelineResource> Timeline);
