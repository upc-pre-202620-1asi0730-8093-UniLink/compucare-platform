namespace CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;

/// <summary>
///     The lifecycle of a service request.
/// </summary>
public enum EServiceRequestStatus
{
    PendingAssignment,
    Assigned,
    InDiagnosis,
    Resolved,
    Cancelled
}
