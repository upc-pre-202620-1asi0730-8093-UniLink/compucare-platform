using System.Net.Mime;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Plan endpoints")]
public class PlansController(IPlanQueryService planQueryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Get all plans")]
    [EndpointDescription("Lists the published subscription plans, cheapest first")]
    [ProducesResponseType(typeof(IEnumerable<PlanResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlans(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var plans = await planQueryService.Handle(new GetAllPlansQuery(), cancellationToken);
        return Ok(plans.Select(PlanResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
