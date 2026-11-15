using CompuCare.Platform.Subscriptions.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;

/// <summary>
///     A published plan of the CompuCare catalog.
/// </summary>
public class Plan : AggregateRoot
{
    public Plan()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Plan(string code, string name, decimal price, decimal hours, int preventiveVisits) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hours);
        ArgumentOutOfRangeException.ThrowIfNegative(preventiveVisits);
        Code = code;
        Name = name;
        Price = price;
        Hours = hours;
        PreventiveVisits = preventiveVisits;
    }

    public PlanId Id { get; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public decimal Hours { get; private set; }
    public int PreventiveVisits { get; private set; }
}
