using CompuCare.Platform.Iam.Domain.Model;
using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Iam.Interfaces.Rest.Transform;

public static class IamActionResultAssembler
{
    private static int ToStatusCodeFromIamError(IamError error)
    {
        return error switch
        {
            IamError.InvalidCredentials => StatusCodes.Status401Unauthorized,
            IamError.InvalidUserData => StatusCodes.Status400BadRequest,
            IamError.InvalidRegistrationData => StatusCodes.Status400BadRequest,
            IamError.InvalidPassword => StatusCodes.Status400BadRequest,
            IamError.CurrentPasswordMismatch => StatusCodes.Status400BadRequest,
            IamError.InvalidPaymentData => StatusCodes.Status400BadRequest,
            IamError.InvalidResetToken => StatusCodes.Status400BadRequest,
            IamError.UserNotFound => StatusCodes.Status404NotFound,
            IamError.PlanNotFound => StatusCodes.Status404NotFound,
            IamError.UserUpdateForbidden => StatusCodes.Status403Forbidden,
            IamError.EmailAlreadyRegistered => StatusCodes.Status409Conflict,
            IamError.CompanyTaxIdAlreadyRegistered => StatusCodes.Status409Conflict,
            IamError.OperationCancelled => StatusCodes.Status409Conflict,
            IamError.EmailDeliveryUnavailable => StatusCodes.Status503ServiceUnavailable,
            IamError.ExternalServiceError => StatusCodes.Status503ServiceUnavailable,
            IamError.DatabaseError => StatusCodes.Status500InternalServerError,
            IamError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = result.Error is IamError iamError ? iamError : IamError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromIamError(error), error,
            errorLocalizer[$"{error}"]);
    }

    public static IActionResult ToActionResultFromGetUserByIdResult(
        ControllerBase controller,
        User? user,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<User, IActionResult> successAction)
    {
        if (user is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromIamError(IamError.UserNotFound),
                IamError.UserNotFound,
                errorLocalizer[nameof(IamError.UserNotFound)]
            );
        return successAction(user);
    }
}
