namespace CompuCare.Platform.Notifications.Interfaces.Acl;

/// <summary>
///     Facade for the Notifications context.
/// </summary>
public interface INotificationsContextFacade
{
    /// <summary>
    ///     Queues an in-app notification for each user. The notifications are tracked but not saved:
    ///     the calling command commits them together with its own changes.
    /// </summary>
    Task Notify(IEnumerable<int> userIds, string message, CancellationToken cancellationToken);
}
