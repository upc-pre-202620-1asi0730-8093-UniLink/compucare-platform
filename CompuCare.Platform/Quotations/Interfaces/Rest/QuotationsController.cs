using System.Net.Mime;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Quotations.Application.CommandServices;
using CompuCare.Platform.Quotations.Application.QueryServices;
using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.Queries;
using CompuCare.Platform.Quotations.Interfaces.Rest.Resources;
using CompuCare.Platform.Quotations.Interfaces.Rest.Transform;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Quotations.Interfaces.Rest;

/// <summary>
///     Quotations are published as "quotes", the name the web application already uses.
/// </summary>
[Authorize(UserRoles.Administrator, UserRoles.CompanyManager, UserRoles.Technician, UserRoles.SystemAdministrator)]
[ApiController]
[Route("api/v1/quotes")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Quotation endpoints")]
public class QuotationsController(
    IQuotationCommandService quotationCommandService,
    IQuotationQueryService quotationQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get my quotations")]
    [EndpointDescription(
        "Administrators see their company's quotations, technicians their own and UniLink support all of them")]
    [ProducesResponseType(typeof(IEnumerable<QuotationResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuotations(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetAuthenticatedUser();
        IEnumerable<Quotation> quotations = user.Role switch
        {
            UserRoles.SystemAdministrator =>
                await quotationQueryService.Handle(new GetAllQuotationsQuery(), cancellationToken),
            UserRoles.Technician =>
                await quotationQueryService.Handle(new GetQuotationsByTechnicianIdQuery(user.UserId),
                    cancellationToken),
            _ => await quotationQueryService.Handle(new GetQuotationsByCompanyIdQuery(user.CompanyId ?? 0),
                cancellationToken)
        };
        return Ok(quotations.Select(QuotationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [Authorize(UserRoles.Technician)]
    [EndpointSummary("Issue a quotation")]
    [EndpointDescription("The assigned technician quotes additional parts or work for a service request")]
    [ProducesResponseType(typeof(QuotationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IssueQuotation([FromBody] IssueQuotationResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var technicianId = HttpContext.GetAuthenticatedUser().UserId;
        var command = IssueQuotationCommandFromResourceAssembler.ToCommandFromResource(resource, technicianId);
        var result = await quotationCommandService.Handle(command, cancellationToken);

        return QuotationsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, quotation => StatusCode(StatusCodes.Status201Created,
                QuotationResourceFromEntityAssembler.ToResourceFromEntity(quotation)));
    }

    [HttpPut("{quotationId:int}/status")]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Approve or reject a quotation")]
    [EndpointDescription("A company administrator approves or rejects a pending quotation of the company")]
    [ProducesResponseType(typeof(QuotationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuotationStatus(int quotationId,
        [FromBody] UpdateQuotationStatusResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = UpdateQuotationStatusCommandFromResourceAssembler.ToCommandFromResource(quotationId, companyId,
            resource);
        var result = await quotationCommandService.Handle(command, cancellationToken);

        return QuotationsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, quotation => Ok(QuotationResourceFromEntityAssembler.ToResourceFromEntity(quotation)));
    }

    [HttpPost("{quotationId:int}/pay")]
    [Authorize(UserRoles.Administrator, UserRoles.CompanyManager)]
    [EndpointSummary("Pay a quotation (simulated)")]
    [EndpointDescription("Records an academic payment of an approved quotation. No card data is stored")]
    [ProducesResponseType(typeof(QuotationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayQuotation(int quotationId, [FromBody] PayQuotationResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = PayQuotationCommandFromResourceAssembler.ToCommandFromResource(quotationId, companyId, resource);
        var result = await quotationCommandService.Handle(command, cancellationToken);

        return QuotationsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, quotation => Ok(QuotationResourceFromEntityAssembler.ToResourceFromEntity(quotation)));
    }
}
