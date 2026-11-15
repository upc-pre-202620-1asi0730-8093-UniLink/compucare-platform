using CompuCare.Platform.Notifications.Domain.Model;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Notifications.Interfaces.Rest.Transform;

public static class NotificationsActionResultAssembler
{
    private static int ToStatusCodeFromNotificationsError(NotificationsError error)
    {
        return error switch
        {
            NotificationsError.NotificationNotFound => StatusCodes.Status404NotFound,
            NotificationsError.OperationCancelled => StatusCodes.Status409Conflict,
            NotificationsError.DatabaseError => StatusCodes.Status500InternalServerError,
            NotificationsError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = result.Error is NotificationsError notificationsError
            ? notificationsError
            : NotificationsError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromNotificationsError(error), error,
            errorLocalizer[$"{error}"]);
    }
}
