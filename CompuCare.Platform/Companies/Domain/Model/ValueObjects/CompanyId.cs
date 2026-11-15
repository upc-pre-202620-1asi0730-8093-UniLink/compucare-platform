using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Companies.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a company.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct CompanyId(int Value) : IEntityId<int>
{
    public static implicit operator int(CompanyId id) => id.Value;
    public static implicit operator CompanyId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
