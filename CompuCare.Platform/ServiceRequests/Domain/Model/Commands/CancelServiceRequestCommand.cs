namespace CompuCare.Platform.ServiceRequests.Domain.Model.Commands;

/// <summary>
///     The requester cancels a service request that was not assigned yet.
/// </summary>
public record CancelServiceRequestCommand(int ServiceRequestId, int RequesterId);
