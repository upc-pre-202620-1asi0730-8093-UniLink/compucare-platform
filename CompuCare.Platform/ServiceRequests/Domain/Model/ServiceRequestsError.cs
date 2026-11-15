namespace CompuCare.Platform.ServiceRequests.Domain.Model;

public enum ServiceRequestsError
{
    None,
    ServiceRequestNotFound,
    InvalidServiceRequestData,
    EquipmentNotAssignedToRequester,
    TechnicianNotFound,
    InvalidStatusTransition,
    InvalidResolutionData,
    UnresolvedQuotations,
    SubscriptionRequired,
    SupportHoursAlreadyRegistered,
    InsufficientSupportHours,
    SubscriptionExpired,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
