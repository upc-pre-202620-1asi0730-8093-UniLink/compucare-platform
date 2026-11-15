using System.Net.Mime;
using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Application.QueryServices;
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
[Tags("Available Location endpoints")]
public class LocationsController(
    ILocationCommandService locationCommandService,
    ILocationQueryService locationQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the locations of my company")]
    [EndpointDescription("Lists the sites and desks of the signed-in user's company")]
    [ProducesResponseType(typeof(IEnumerable<LocationResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var locations = await locationQueryService.Handle(new GetLocationsByCompanyIdQuery(companyId),
            cancellationToken);
        return Ok(locations.Select(LocationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Create a location")]
    [EndpointDescription("Registers a site and desk for the signed-in administrator's company")]
    [ProducesResponseType(typeof(LocationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = CreateLocationCommandFromResourceAssembler.ToCommandFromResource(resource, companyId);
        var result = await locationCommandService.Handle(command, cancellationToken);

        return CompaniesActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, location => StatusCode(StatusCodes.Status201Created,
                LocationResourceFromEntityAssembler.ToResourceFromEntity(location)));
    }

    [HttpPatch("{locationId:int}")]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Update a location")]
    [EndpointDescription("Renames, activates or deactivates a location of the signed-in administrator's company")]
    [ProducesResponseType(typeof(LocationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(int locationId, [FromBody] UpdateLocationResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = UpdateLocationCommandFromResourceAssembler.ToCommandFromResource(locationId, companyId,
            resource);
        var result = await locationCommandService.Handle(command, cancellationToken);

        return CompaniesActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, location => Ok(LocationResourceFromEntityAssembler.ToResourceFromEntity(location)));
    }
}
