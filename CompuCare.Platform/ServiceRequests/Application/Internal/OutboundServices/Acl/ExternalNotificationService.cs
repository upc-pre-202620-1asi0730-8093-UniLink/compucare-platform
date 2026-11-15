using CompuCare.Platform.Notifications.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Service Requests bounded context to send in-app notifications.
/// </summary>
/// <param name="notificationsContextFacade">Notifications bounded-context facade.</param>
public class ExternalNotificationService(INotificationsContextFacade notificationsContextFacade)
{
    public async Task Notify(IEnumerable<int> userIds, string message, CancellationToken cancellationToken)
    {
        await notificationsContextFacade.Notify(userIds, message, cancellationToken);
    }
}
