using System.Net.Mime;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Notifications.Application.CommandServices;
using CompuCare.Platform.Notifications.Application.QueryServices;
using CompuCare.Platform.Notifications.Domain.Model.Commands;
using CompuCare.Platform.Notifications.Domain.Model.Queries;
using CompuCare.Platform.Notifications.Interfaces.Rest.Resources;
using CompuCare.Platform.Notifications.Interfaces.Rest.Transform;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CompuCare.Platform.Notifications.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Notification endpoints")]
public class NotificationsController(
    INotificationCommandService notificationCommandService,
    INotificationQueryService notificationQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get my notifications")]
    [EndpointDescription("Lists the in-app notifications of the signed-in user, latest first")]
    [ProducesResponseType(typeof(IEnumerable<NotificationResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetAuthenticatedUser().UserId;
        var notifications = await notificationQueryService.Handle(new GetNotificationsByUserIdQuery(userId),
            cancellationToken);
        return Ok(notifications.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{notificationId:int}/read")]
    [EndpointSummary("Mark a notification as read")]
    [EndpointDescription("Marks one notification of the signed-in user as read")]
    [ProducesResponseType(typeof(NotificationReadResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int notificationId,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetAuthenticatedUser().UserId;
        var result = await notificationCommandService.Handle(new MarkNotificationAsReadCommand(notificationId, userId),
            cancellationToken);

        return NotificationsActionResultAssembler.ToActionResultFromResult(this, result, errorLocalizer,
            problemDetailsFactory, notification => Ok(new NotificationReadResource(notification.Id, notification.Read)));
    }
}
