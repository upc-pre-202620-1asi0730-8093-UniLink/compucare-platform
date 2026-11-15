using CompuCare.Platform.Notifications.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Notifications.Domain.Model.Aggregates;

/// <summary>
///     An in-app message for one user. It is not proof that an email was delivered.
/// </summary>
public partial class Notification : AggregateRoot
{
    public Notification()
    {
        Message = string.Empty;
    }

    public Notification(int userId, string message) : this()
    {
        if (userId <= 0)
            throw new ArgumentException("A valid user is required.", nameof(userId));
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        UserId = userId;
        Message = message.Trim();
    }

    public NotificationId Id { get; }
    public int UserId { get; private set; }
    public string Message { get; private set; }
    public bool Read { get; private set; }

    public Notification MarkAsRead()
    {
        Read = true;
        return this;
    }
}
