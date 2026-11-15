using CompuCare.Platform.Notifications.Application.CommandServices;
using CompuCare.Platform.Notifications.Domain.Model;
using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Model.Commands;
using CompuCare.Platform.Notifications.Domain.Model.Errors;
using CompuCare.Platform.Notifications.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Notifications.Application.Internal.CommandServices;

public class NotificationCommandService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    : INotificationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Notification>> Handle(MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId, cancellationToken);
        if (notification is null || notification.UserId != command.UserId)
            return Result<Notification>.Failure(NotificationsErrors.NotificationNotFound,
                NotificationsError.NotificationNotFound);

        notification.MarkAsRead();
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Notification>.Success(notification);
    }
}
