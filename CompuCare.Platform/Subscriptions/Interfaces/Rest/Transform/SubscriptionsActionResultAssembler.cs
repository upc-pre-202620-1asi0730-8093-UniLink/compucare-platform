using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using CompuCare.Platform.Subscriptions.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class SubscriptionsActionResultAssembler
{
    private static int ToStatusCodeFromSubscriptionsError(SubscriptionsError error)
    {
        return error switch
        {
            SubscriptionsError.InvalidBillingData => StatusCodes.Status400BadRequest,
            SubscriptionsError.InvalidMaintenanceData => StatusCodes.Status400BadRequest,
            SubscriptionsError.MaintenanceDateInPast => StatusCodes.Status400BadRequest,
            SubscriptionsError.MaintenanceEquipmentNotInCompany => StatusCodes.Status400BadRequest,
            SubscriptionsError.PlanNotFound => StatusCodes.Status404NotFound,
            SubscriptionsError.SubscriptionNotFound => StatusCodes.Status404NotFound,
            SubscriptionsError.SubscriptionExpired => StatusCodes.Status409Conflict,
            SubscriptionsError.NoPreventiveVisitsLeft => StatusCodes.Status409Conflict,
            SubscriptionsError.OperationCancelled => StatusCodes.Status409Conflict,
            SubscriptionsError.DatabaseError => StatusCodes.Status500InternalServerError,
            SubscriptionsError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = result.Error is SubscriptionsError subscriptionsError
            ? subscriptionsError
            : SubscriptionsError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromSubscriptionsError(error), error,
            errorLocalizer[$"{error}"]);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? value,
        SubscriptionsError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction) where T : class
    {
        if (value is null)
            return problemDetailsFactory.CreateProblemDetails(controller,
                ToStatusCodeFromSubscriptionsError(notFoundError), notFoundError, errorLocalizer[$"{notFoundError}"]);
        return successAction(value);
    }
}
