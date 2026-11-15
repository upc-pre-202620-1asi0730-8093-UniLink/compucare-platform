using System.Net.Mime;
using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Application.QueryServices;
using CompuCare.Platform.Companies.Domain.Model;
using CompuCare.Platform.Companies.Domain.Model.Queries;
using CompuCare.Platform.Companies.Interfaces.Rest.Resources;
using CompuCare.Platform.Companies.Interfaces.Rest.Transform;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Companies.Interfaces.Rest;

[Authorize(UserRoles.Administrator, UserRoles.CompanyManager, UserRoles.Employee)]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Equipment endpoints")]
public class EquipmentsController(
    IEquipmentCommandService equipmentCommandService,
    IEquipmentQueryService equipmentQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the equipment of my company")]
    [EndpointDescription(
        "Administrators see every device of their company; employees only see the devices assigned to them")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipments(
        [FromQuery] string? status,
        [FromQuery] string? type,
        [FromQuery] int? locationId,
        [FromQuery] string? q,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetAuthenticatedUser();
        var statusFilter = EquipmentStatusLabelAssembler.ToStatusFromLabel(status);
        if (!string.IsNullOrWhiteSpace(status) && statusFilter is null)
            return Ok(Array.Empty<EquipmentResource>());

        var query = new GetEquipmentsByCompanyIdQuery(
            user.CompanyId ?? 0,
            user.Role == UserRoles.Employee ? user.UserId : null,
            statusFilter,
            type,
            locationId,
            q);
        var equipments = await equipmentQueryService.Handle(query, cancellationToken);
        return Ok(equipments.Select(EquipmentResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Register a device")]
    [EndpointDescription("Registers a device for the signed-in administrator's company")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = CreateEquipmentCommandFromResourceAssembler.ToCommandFromResource(resource, companyId);
        var result = await equipmentCommandService.Handle(command, cancellationToken);

        return CompaniesActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, equipment => StatusCode(StatusCodes.Status201Created,
                EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment)));
    }

    [HttpPut("{equipmentId:int}/assign")]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Assign a device")]
    [EndpointDescription("Assigns a device to an employee of the company, or unassigns it with a null employee")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignEquipment(int equipmentId, [FromBody] AssignEquipmentResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = AssignEquipmentCommandFromResourceAssembler.ToCommandFromResource(equipmentId, companyId,
            resource);
        var result = await equipmentCommandService.Handle(command, cancellationToken);

        return CompaniesActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, equipment => Ok(EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment)));
    }

    [HttpGet("{equipmentId:int}/history")]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Get the history of a device")]
    [EndpointDescription("Lists the assignments, service requests, quotations and maintenance visits of a device")]
    [ProducesResponseType(typeof(EquipmentHistoryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEquipmentHistory(int equipmentId,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var history = await equipmentQueryService.Handle(new GetEquipmentHistoryQuery(equipmentId, companyId),
            cancellationToken);

        return CompaniesActionResultAssembler.ToActionResultFromNullable(this, history,
            CompaniesError.EquipmentNotFound, errorLocalizer, problemDetailsFactory,
            found => Ok(EquipmentHistoryResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }
}
