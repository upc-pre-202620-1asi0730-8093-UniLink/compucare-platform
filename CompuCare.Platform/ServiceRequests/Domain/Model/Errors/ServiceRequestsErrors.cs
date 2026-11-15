using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.ServiceRequests.Domain.Model.Errors;

public static class ServiceRequestsErrors
{
    public static readonly Error ServiceRequestNotFound =
        Error.NotFound("ServiceRequests.ServiceRequestNotFound", "The specified service request was not found.");

    public static readonly Error InvalidServiceRequestData =
        Error.Validation("ServiceRequests.InvalidServiceRequestData",
            "A category and a description of at least ten characters are required.");

    public static readonly Error EquipmentNotAssignedToRequester =
        Error.Forbidden("ServiceRequests.EquipmentNotAssignedToRequester", "This device is not assigned to you.");

    public static readonly Error TechnicianNotFound =
        Error.Validation("ServiceRequests.TechnicianNotFound", "The specified technician was not found.");

    public static readonly Error InvalidStatusTransition =
        Error.Conflict("ServiceRequests.InvalidStatusTransition",
            "The service request does not allow this operation in its current status.");

    public static readonly Error InvalidResolutionData =
        Error.Validation("ServiceRequests.InvalidResolutionData",
            "Positive work hours, a diagnosis and the completed work are required.");

    public static readonly Error UnresolvedQuotations =
        Error.Conflict("ServiceRequests.UnresolvedQuotations",
            "Required quotations must be resolved before completing a ticket.");

    public static readonly Error SubscriptionRequired =
        Error.Conflict("ServiceRequests.SubscriptionRequired",
            "An active subscription is required to complete service requests.");

    public static readonly Error SupportHoursAlreadyRegistered =
        Error.Conflict("ServiceRequests.SupportHoursAlreadyRegistered",
            "Support hours have already been registered for this ticket.");

    public static readonly Error InsufficientSupportHours =
        Error.Conflict("ServiceRequests.InsufficientSupportHours",
            "Insufficient support hours in the current subscription period.");

    public static readonly Error SubscriptionExpired =
        Error.Conflict("ServiceRequests.SubscriptionExpired", "The subscription period has expired. Renew it first.");
}
