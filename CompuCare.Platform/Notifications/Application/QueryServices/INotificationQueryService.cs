using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Model.Queries;

namespace CompuCare.Platform.Notifications.Application.QueryServices;

/// <summary>
///     The notification query service.
/// </summary>
public interface INotificationQueryService
{
    Task<IEnumerable<Notification>> Handle(GetNotificationsByUserIdQuery query, CancellationToken cancellationToken);
}
