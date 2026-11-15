using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.ServiceRequests.Domain.Model;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;

public static class ServiceRequestsActionResultAssembler
{
    private static int ToStatusCodeFromServiceRequestsError(ServiceRequestsError error)
    {
        return error switch
        {
            ServiceRequestsError.InvalidServiceRequestData => StatusCodes.Status400BadRequest,
            ServiceRequestsError.TechnicianNotFound => StatusCodes.Status400BadRequest,
            ServiceRequestsError.InvalidResolutionData => StatusCodes.Status400BadRequest,
            ServiceRequestsError.EquipmentNotAssignedToRequester => StatusCodes.Status403Forbidden,
            ServiceRequestsError.ServiceRequestNotFound => StatusCodes.Status404NotFound,
            ServiceRequestsError.InvalidStatusTransition => StatusCodes.Status409Conflict,
            ServiceRequestsError.UnresolvedQuotations => StatusCodes.Status409Conflict,
            ServiceRequestsError.SubscriptionRequired => StatusCodes.Status409Conflict,
            ServiceRequestsError.SupportHoursAlreadyRegistered => StatusCodes.Status409Conflict,
            ServiceRequestsError.InsufficientSupportHours => StatusCodes.Status409Conflict,
            ServiceRequestsError.SubscriptionExpired => StatusCodes.Status409Conflict,
            ServiceRequestsError.OperationCancelled => StatusCodes.Status409Conflict,
            ServiceRequestsError.DatabaseError => StatusCodes.Status500InternalServerError,
            ServiceRequestsError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult<T>(
        ControllerBase controller,
        Result<T> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var error = result.Error is ServiceRequestsError serviceRequestsError
            ? serviceRequestsError
            : ServiceRequestsError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromServiceRequestsError(error),
            error, errorLocalizer[$"{error}"]);
    }
}
