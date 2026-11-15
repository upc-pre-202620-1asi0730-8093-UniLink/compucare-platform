namespace CompuCare.Platform.ServiceRequests.Domain.Model.Commands;

/// <summary>
///     A UniLink administrator assigns a technician to a pending service request.
/// </summary>
public record AssignTechnicianCommand(int ServiceRequestId, int TechnicianId);
