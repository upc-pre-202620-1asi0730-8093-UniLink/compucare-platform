namespace CompuCare.Platform.ServiceRequests.Domain.Model.Commands;

/// <summary>
///     The assigned technician starts diagnosing a service request.
/// </summary>
public record StartDiagnosisCommand(int ServiceRequestId, int TechnicianId);
