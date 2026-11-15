using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a piece of equipment.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct EquipmentId(int Value) : IEntityId<int>
{
    public static implicit operator int(EquipmentId id) => id.Value;
    public static implicit operator EquipmentId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
