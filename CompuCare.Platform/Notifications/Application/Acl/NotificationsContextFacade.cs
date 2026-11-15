using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Repositories;
using CompuCare.Platform.Notifications.Interfaces.Acl;

namespace CompuCare.Platform.Notifications.Application.Acl;

/// <summary>
///     Facade for the Notifications context.
/// </summary>
public class NotificationsContextFacade(INotificationRepository notificationRepository)
    : INotificationsContextFacade
{
    /// <inheritdoc />
    public async Task Notify(IEnumerable<int> userIds, string message, CancellationToken cancellationToken)
    {
        foreach (var userId in userIds.Distinct())
            await notificationRepository.AddAsync(new Notification(userId, message), cancellationToken);
    }
}
