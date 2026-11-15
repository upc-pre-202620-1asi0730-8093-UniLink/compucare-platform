using System.Net.Mime;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
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
[Tags("Available Subscription endpoints")]
public class SubscriptionsController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet("balance")]
    [EndpointSummary("Get the coverage balance of my company")]
    [EndpointDescription("Returns the support hours and preventive visits used and left in the current period")]
    [ProducesResponseType(typeof(BalanceResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var subscription = await subscriptionQueryService.Handle(new GetSubscriptionByCompanyIdQuery(companyId),
            cancellationToken);

        return SubscriptionsActionResultAssembler.ToActionResultFromNullable(this, subscription,
            SubscriptionsError.SubscriptionNotFound, errorLocalizer, problemDetailsFactory,
            found => Ok(BalanceResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [EndpointSummary("Subscribe to a plan")]
    [EndpointDescription("Subscribes my company to a plan, or switches the current subscription to it")]
    [ProducesResponseType(typeof(BalanceResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var command = SubscribeCommandFromResourceAssembler.ToCommandFromResource(resource, companyId);
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return SubscriptionsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, subscription => StatusCode(StatusCodes.Status201Created,
                BalanceResourceFromEntityAssembler.ToResourceFromEntity(subscription)));
    }

    [HttpPost("renew")]
    [EndpointSummary("Renew my subscription")]
    [EndpointDescription("Starts a new billing period of the current plan")]
    [ProducesResponseType(typeof(BalanceResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Renew(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var companyId = HttpContext.GetAuthenticatedUser().CompanyId ?? 0;
        var result = await subscriptionCommandService.Handle(new RenewSubscriptionCommand(companyId),
            cancellationToken);

        return SubscriptionsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, subscription => Ok(BalanceResourceFromEntityAssembler.ToResourceFromEntity(subscription)));
    }
}
