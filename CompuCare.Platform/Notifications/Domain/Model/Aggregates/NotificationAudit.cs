using CompuCare.Platform.Shared.Domain.Model.Entities;

namespace CompuCare.Platform.Notifications.Domain.Model.Aggregates;

public partial class Notification : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
