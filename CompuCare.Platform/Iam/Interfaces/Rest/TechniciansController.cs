using System.Net.Mime;
using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Model.Queries;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;
using CompuCare.Platform.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace CompuCare.Platform.Iam.Interfaces.Rest;

[Authorize(UserRoles.SystemAdministrator)]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Technician endpoints")]
public class TechniciansController(IUserQueryService userQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all active technicians")]
    [EndpointDescription("Lists the active UniLink technicians that can be assigned to service requests")]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveTechnicians(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var technicians = await userQueryService.Handle(new GetAllActiveTechniciansQuery(), cancellationToken);
        return Ok(technicians.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
