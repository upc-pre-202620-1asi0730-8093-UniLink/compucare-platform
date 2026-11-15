namespace CompuCare.Platform.Notifications.Domain.Model.Commands;

/// <summary>
///     Marks a notification of the signed-in user as read.
/// </summary>
public record MarkNotificationAsReadCommand(int NotificationId, int UserId);
