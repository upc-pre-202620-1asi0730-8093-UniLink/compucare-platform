using CompuCare.Platform.Notifications.Application.QueryServices;
using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Model.Queries;
using CompuCare.Platform.Notifications.Domain.Repositories;

namespace CompuCare.Platform.Notifications.Application.Internal.QueryServices;

public class NotificationQueryService(INotificationRepository notificationRepository) : INotificationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Notification>> Handle(GetNotificationsByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await notificationRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}
