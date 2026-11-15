namespace CompuCare.Platform.ServiceRequests.Domain.Model.Commands;

/// <summary>
///     The assigned technician documents the work and closes a service request.
/// </summary>
public record ResolveServiceRequestCommand(int ServiceRequestId, int TechnicianId, string Diagnosis, string WorkDone,
    decimal Hours);
