using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Subscriptions.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a preventive maintenance visit.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct MaintenanceVisitId(int Value) : IEntityId<int>
{
    public static implicit operator int(MaintenanceVisitId id) => id.Value;
    public static implicit operator MaintenanceVisitId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
