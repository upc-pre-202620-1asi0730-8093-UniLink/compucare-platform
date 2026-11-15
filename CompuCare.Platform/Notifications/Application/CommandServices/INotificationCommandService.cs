using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Notifications.Application.CommandServices;

/// <summary>
///     The notification command service.
/// </summary>
public interface INotificationCommandService
{
    Task<Result<Notification>> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken);
}
