using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Notifications.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for an in-app notification.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct NotificationId(int Value) : IEntityId<int>
{
    public static implicit operator int(NotificationId id) => id.Value;
    public static implicit operator NotificationId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
