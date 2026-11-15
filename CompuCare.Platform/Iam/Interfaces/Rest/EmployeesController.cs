using System.Net.Mime;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Model.Queries;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;
using CompuCare.Platform.Iam.Interfaces.Rest.Transform;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Iam.Interfaces.Rest;

[Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Employee endpoints")]
public class EmployeesController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the employees of my company")]
    [EndpointDescription("Lists the employee accounts of the signed-in administrator's company")]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var employees = await userQueryService.Handle(new GetEmployeesByCompanyIdQuery(companyId), cancellationToken);
        return Ok(employees.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Create an employee")]
    [EndpointDescription("Creates an employee account with a temporary password sent by email")]
    [ProducesResponseType(typeof(EmployeeResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = CreateEmployeeCommandFromResourceAssembler.ToCommandFromResource(resource, companyId);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            created => StatusCode(StatusCodes.Status201Created,
                EmployeeResourceFromEntityAssembler.ToResourceFromEntity(created.employee,
                    created.temporaryPassword)));
    }
}
