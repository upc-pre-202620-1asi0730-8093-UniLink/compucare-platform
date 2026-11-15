using System.Net.Mime;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest;

[Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Maintenance endpoints")]
public class MaintenancesController(
    IMaintenanceVisitCommandService maintenanceVisitCommandService,
    IMaintenanceVisitQueryService maintenanceVisitQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the maintenance visits of my company")]
    [EndpointDescription("Lists the preventive maintenance visits booked by my company, latest first")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaintenances(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var visits = await maintenanceVisitQueryService.Handle(new GetMaintenanceVisitsByCompanyIdQuery(companyId),
            cancellationToken);
        return Ok(visits.Select(MaintenanceResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Schedule a maintenance visit")]
    [EndpointDescription("Books a preventive visit of the current period for some devices of my company")]
    [ProducesResponseType(typeof(MaintenanceResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ScheduleMaintenance([FromBody] ScheduleMaintenanceResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = ScheduleMaintenanceVisitCommandFromResourceAssembler.ToCommandFromResource(resource, companyId);
        var result = await maintenanceVisitCommandService.Handle(command, cancellationToken);

        return SubscriptionsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, visit => StatusCode(StatusCodes.Status201Created,
                MaintenanceResourceFromEntityAssembler.ToResourceFromEntity(visit)));
    }
}
