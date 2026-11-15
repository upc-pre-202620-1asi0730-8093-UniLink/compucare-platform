using CompuCare.Platform.Shared.Domain.Model.ValueObjects;

namespace CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a service request (ticket).
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct ServiceRequestId(int Value) : IEntityId<int>
{
    public static implicit operator int(ServiceRequestId id) => id.Value;
    public static implicit operator ServiceRequestId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
