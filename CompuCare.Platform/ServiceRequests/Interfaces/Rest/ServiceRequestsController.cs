using System.Net.Mime;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.ServiceRequests.Application.CommandServices;
using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Application.QueryServices;
using CompuCare.Platform.ServiceRequests.Domain.Model.Commands;
using CompuCare.Platform.ServiceRequests.Domain.Model.Queries;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest;

/// <summary>
///     Service requests are published as "tickets", the name the web application already uses.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/tickets")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Service Request endpoints")]
public class ServiceRequestsController(
    IServiceRequestCommandService serviceRequestCommandService,
    IServiceRequestQueryService serviceRequestQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get my service requests")]
    [EndpointDescription(
        "Employees see their own requests, technicians the assigned ones, administrators their company's and UniLink support all of them")]
    [ProducesResponseType(typeof(IEnumerable<ServiceRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServiceRequests(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetAuthenticatedUser();
        IEnumerable<ServiceRequestDetails> serviceRequests = user.Role switch
        {
            UserRoles.SystemAdministrator =>
                await serviceRequestQueryService.Handle(new GetAllServiceRequestsQuery(), cancellationToken),
            UserRoles.Administrator or UserRoles.CompanyManager =>
                await serviceRequestQueryService.Handle(new GetServiceRequestsByCompanyIdQuery(user.CompanyId ?? 0),
                    cancellationToken),
            UserRoles.Employee =>
                await serviceRequestQueryService.Handle(new GetServiceRequestsByRequesterIdQuery(user.UserId),
                    cancellationToken),
            UserRoles.Technician =>
                await serviceRequestQueryService.Handle(new GetServiceRequestsByTechnicianIdQuery(user.UserId),
                    cancellationToken),
            _ => []
        };
        return Ok(serviceRequests.Select(ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [Authorize(UserRoles.Employee)]
    [EndpointSummary("Report an issue")]
    [EndpointDescription("Reports an issue with one of the devices assigned to the signed-in employee")]
    [ProducesResponseType(typeof(ServiceRequestResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReportServiceRequest([FromBody] ReportServiceRequestResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetAuthenticatedUser();
        var command = ReportServiceRequestCommandFromResourceAssembler.ToCommandFromResource(resource,
            user.CompanyId ?? 0, user.UserId);
        var result = await serviceRequestCommandService.Handle(command, cancellationToken);

        return ServiceRequestsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, details => StatusCode(StatusCodes.Status201Created,
                ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(details)));
    }

    [HttpPut("{serviceRequestId:int}/cancel")]
    [Authorize(UserRoles.Employee)]
    [EndpointSummary("Cancel a service request")]
    [EndpointDescription("The requester cancels a service request before it is assigned")]
    [ProducesResponseType(typeof(ServiceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelServiceRequest(int serviceRequestId,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var requesterId = HttpContext.GetAuthenticatedUser().UserId;
        var result = await serviceRequestCommandService.Handle(
            new CancelServiceRequestCommand(serviceRequestId, requesterId), cancellationToken);

        return ServiceRequestsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, details => Ok(ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(details)));
    }

    [HttpPut("{serviceRequestId:int}/assign")]
    [Authorize(UserRoles.SystemAdministrator)]
    [EndpointSummary("Assign a technician")]
    [EndpointDescription("UniLink support assigns an active technician to a pending service request")]
    [ProducesResponseType(typeof(ServiceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTechnician(int serviceRequestId,
        [FromBody] AssignTechnicianResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var command = AssignTechnicianCommandFromResourceAssembler.ToCommandFromResource(serviceRequestId, resource);
        var result = await serviceRequestCommandService.Handle(command, cancellationToken);

        return ServiceRequestsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, details => Ok(ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(details)));
    }

    [HttpPut("{serviceRequestId:int}/start")]
    [Authorize(UserRoles.Technician)]
    [EndpointSummary("Start the diagnosis")]
    [EndpointDescription("The assigned technician starts diagnosing the service request")]
    [ProducesResponseType(typeof(ServiceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartDiagnosis(int serviceRequestId,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var technicianId = HttpContext.GetAuthenticatedUser().UserId;
        var result = await serviceRequestCommandService.Handle(
            new StartDiagnosisCommand(serviceRequestId, technicianId), cancellationToken);

        return ServiceRequestsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, details => Ok(ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(details)));
    }

    [HttpPut("{serviceRequestId:int}/diagnosis")]
    [Authorize(UserRoles.Technician)]
    [EndpointSummary("Resolve a service request")]
    [EndpointDescription(
        "The assigned technician documents the diagnosis and the work done, consuming support hours of the company")]
    [ProducesResponseType(typeof(ServiceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResolveServiceRequest(int serviceRequestId,
        [FromBody] ResolveServiceRequestResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var technicianId = HttpContext.GetAuthenticatedUser().UserId;
        var command = ResolveServiceRequestCommandFromResourceAssembler.ToCommandFromResource(serviceRequestId,
            technicianId, resource);
        var result = await serviceRequestCommandService.Handle(command, cancellationToken);

        return ServiceRequestsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, details => Ok(ServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(details)));
    }
}
