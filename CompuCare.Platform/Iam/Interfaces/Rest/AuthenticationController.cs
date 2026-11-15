using System.Net.Mime;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Model.Queries;
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
[Route("api/v1/auth")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Authentication endpoints")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IPasswordResetCommandService passwordResetCommandService,
    IUserQueryService userQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Sign in")]
    [EndpointDescription("Authenticates the credentials of an active account and issues a JWT bearer token")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(signInCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            userAndToken => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(userAndToken.user,
                userAndToken.token)));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EndpointSummary("Register a company")]
    [EndpointDescription("Registers a company, its first administrator and, optionally, its subscription plan")]
    [ProducesResponseType(typeof(RegisteredCompanyResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var signUpCommand = SignUpCompanyCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(signUpCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            registration => StatusCode(StatusCodes.Status201Created,
                RegisteredCompanyResourceFromEntityAssembler.ToResourceFromEntity(registration.admin,
                    registration.companyName, registration.companyTaxId)));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EndpointSummary("Request a password reset")]
    [EndpointDescription("Issues a one-hour password reset token when the account exists")]
    [ProducesResponseType(typeof(PasswordResetRequestedResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var command = RequestPasswordResetCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordResetCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            devToken => Ok(new PasswordResetRequestedResource(
                "If the account exists, recovery instructions were sent.", devToken)));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EndpointSummary("Reset a password")]
    [EndpointDescription("Sets a new password with a valid password reset token")]
    [ProducesResponseType(typeof(PasswordUpdatedResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var command = ResetPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await passwordResetCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer, problemDetailsFactory,
            _ => Ok(new PasswordUpdatedResource("Password updated.")));
    }

    [HttpGet("me")]
    [EndpointSummary("Get the signed-in user")]
    [EndpointDescription("Returns the account of the bearer token owner")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSignedInUser(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var authenticatedUser = HttpContext.GetAuthenticatedUser();
        var user = await userQueryService.Handle(new GetUserByIdQuery(authenticatedUser.UserId), cancellationToken);

        return IamActionResultAssembler.ToActionResultFromGetUserByIdResult(this, user, errorLocalizer,
            problemDetailsFactory, foundUser => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(foundUser)));
    }
}
