using CompuCare.Platform.Companies.Domain.Model;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Companies.Interfaces.Rest.Transform;

public static class CompaniesActionResultAssembler
{
    private static int ToStatusCodeFromCompaniesError(CompaniesError error)
    {
        return error switch
        {
            CompaniesError.InvalidCompanyData => StatusCodes.Status400BadRequest,
            CompaniesError.InvalidLocationData => StatusCodes.Status400BadRequest,
            CompaniesError.InvalidEquipmentData => StatusCodes.Status400BadRequest,
            CompaniesError.LocationUnavailable => StatusCodes.Status400BadRequest,
            CompaniesError.EmployeeNotInCompany => StatusCodes.Status400BadRequest,
            CompaniesError.LocationNotFound => StatusCodes.Status404NotFound,
            CompaniesError.EquipmentNotFound => StatusCodes.Status404NotFound,
            CompaniesError.DuplicateLocation => StatusCodes.Status409Conflict,
            CompaniesError.DuplicateSerialNumber => StatusCodes.Status409Conflict,
            CompaniesError.OperationCancelled => StatusCodes.Status409Conflict,
            CompaniesError.DatabaseError => StatusCodes.Status500InternalServerError,
            CompaniesError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = result.Error is CompaniesError companiesError ? companiesError : CompaniesError.InternalServerError;
        return problemDetailsFactory.CreateProblemDetails(controller, ToStatusCodeFromCompaniesError(error), error,
            errorLocalizer[$"{error}"]);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? value,
        CompaniesError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction) where T : class
    {
        if (value is null)
            return problemDetailsFactory.CreateProblemDetails(controller,
                ToStatusCodeFromCompaniesError(notFoundError), notFoundError, errorLocalizer[$"{notFoundError}"]);
        return successAction(value);
    }
}
