using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a company location.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct LocationId(int Value) : IEntityId<int>
{
    public static implicit operator int(LocationId id) => id.Value;
    public static implicit operator LocationId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
