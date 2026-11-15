using CompuCare.Platform.Quotations.Domain.Model;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Quotations.Interfaces.Rest.Transform;

public static class QuotationsActionResultAssembler
{
    private static int ToStatusCodeFromQuotationsError(QuotationsError error)
    {
        return error switch
        {
            QuotationsError.InvalidQuotationData => StatusCodes.Status400BadRequest,
            QuotationsError.InvalidQuotationStatus => StatusCodes.Status400BadRequest,
            QuotationsError.InvalidPaymentData => StatusCodes.Status400BadRequest,
            QuotationsError.QuotationNotFound => StatusCodes.Status404NotFound,
            QuotationsError.ServiceRequestNotFound => StatusCodes.Status404NotFound,
            QuotationsError.QuotationNotAllowed => StatusCodes.Status409Conflict,
            QuotationsError.QuotationNotPending => StatusCodes.Status409Conflict,
            QuotationsError.QuotationNotApproved => StatusCodes.Status409Conflict,
            QuotationsError.OperationCancelled => StatusCodes.Status409Conflict,
            QuotationsError.DatabaseError => StatusCodes.Status500InternalServerError,
            QuotationsError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = result.Error is QuotationsError quotationsError ? quotationsError : QuotationsError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromQuotationsError(error), error,
            errorLocalizer[$"{error}"]);
    }
}
