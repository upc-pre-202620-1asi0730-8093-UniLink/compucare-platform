namespace CompuCare.Platform.Notifications.Interfaces.Rest.Resources;

public record NotificationResource(int Id, int UserId, string Message, DateTimeOffset? CreatedAt, bool Read);
