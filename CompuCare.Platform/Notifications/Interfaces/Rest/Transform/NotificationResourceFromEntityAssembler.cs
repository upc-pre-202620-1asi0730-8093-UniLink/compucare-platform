using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Notifications.Interfaces.Rest.Transform;

public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return new NotificationResource(notification.Id, notification.UserId, notification.Message,
            notification.CreatedAt, notification.Read);
    }
}
