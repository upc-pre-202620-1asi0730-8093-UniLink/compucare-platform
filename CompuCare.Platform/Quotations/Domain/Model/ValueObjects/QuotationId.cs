using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.Quotations.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a quotation.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct QuotationId(int Value) : IEntityId<int>
{
    public static implicit operator int(QuotationId id) => id.Value;
    public static implicit operator QuotationId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
