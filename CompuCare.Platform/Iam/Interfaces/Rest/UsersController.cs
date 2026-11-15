using System.Net.Mime;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Iam.Interfaces.Rest.Resources;
using CompuCare.Platform.Iam.Interfaces.Rest.Transform;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available User endpoints")]
public class UsersController(
    IUserCommandService userCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPatch("{userId:int}")]
    [EndpointSummary("Update my profile")]
    [EndpointDescription("Updates the name and phone of the signed-in user")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UpdateUserProfileResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var requesterId = HttpContext.GetAuthenticatedUser().UserId;
        var command = UpdateUserProfileCommandFromResourceAssembler.ToCommandFromResource(userId, requesterId, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            user => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user)));
    }

    [HttpPut("{userId:int}/password")]
    [EndpointSummary("Change my password")]
    [EndpointDescription("Changes the password of the signed-in user after verifying the current one")]
    [ProducesResponseType(typeof(PasswordUpdatedResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeUserPassword(int userId, [FromBody] ChangeUserPasswordResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var requesterId = HttpContext.GetAuthenticatedUser().UserId;
        var command = ChangeUserPasswordCommandFromResourceAssembler.ToCommandFromResource(userId, requesterId, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            _ => Ok(new PasswordUpdatedResource("Password updated.")));
    }
}
