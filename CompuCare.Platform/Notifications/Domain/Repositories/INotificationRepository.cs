using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Notifications.Domain.Repositories;

/// <summary>
///     The notification repository.
/// </summary>
public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IEnumerable<Notification>> FindByUserIdAsync(int userId, CancellationToken cancellationToken);
}
