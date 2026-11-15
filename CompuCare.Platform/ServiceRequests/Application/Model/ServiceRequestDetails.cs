using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;

namespace CompuCare.Platform.ServiceRequests.Application.Model;

/// <summary>
///     A service request together with the display names owned by the Companies and IAM contexts.
/// </summary>
public record ServiceRequestDetails(
    ServiceRequest ServiceRequest,
    string? EquipmentName,
    string? EmployeeName,
    string? TechnicianName);
