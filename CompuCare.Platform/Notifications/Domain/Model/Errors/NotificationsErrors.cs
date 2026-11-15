using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.Notifications.Domain.Model.Errors;

public static class NotificationsErrors
{
    public static readonly Error NotificationNotFound =
        Error.NotFound("Notifications.NotificationNotFound", "The specified notification was not found.");
}
