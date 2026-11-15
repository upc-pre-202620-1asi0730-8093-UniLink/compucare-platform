using CompuCare.Platform.Subscriptions.Domain.Model.Entities;
using CompuCare.Platform.Subscriptions.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Domain.Model.Aggregates;

namespace CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;

/// <summary>
///     The subscription aggregate root. Support hours and preventive visits are consumed here.
/// </summary>
public class Subscription : AggregateRoot
{
    private readonly List<CoverageConsumption> _consumptions = [];

    public Subscription()
    {
        PlanName = string.Empty;
        Status = "active";
    }

    public Subscription(int companyId, Plan plan) : this()
    {
        if (companyId <= 0)
            throw new ArgumentException("A valid company is required.", nameof(companyId));
        ArgumentNullException.ThrowIfNull(plan);
        CompanyId = companyId;
        ApplyPlan(plan);
    }

    public SubscriptionId Id { get; }
    public int CompanyId { get; private set; }
    public PlanId PlanId { get; private set; }
    public string PlanName { get; private set; }
    public decimal MonthlyHours { get; private set; }
    public decimal UsedHours { get; private set; }
    public int PreventiveTotal { get; private set; }
    public int PreventiveUsed { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime RenewsAt { get; private set; }
    public string Status { get; private set; }

    /// <summary>
    ///     Optimistic concurrency token, renewed on every change of the coverage balance.
    /// </summary>
    public Guid Version { get; private set; }

    public IReadOnlyCollection<CoverageConsumption> Consumptions => _consumptions.AsReadOnly();

    public decimal AvailableHours => Math.Max(0, MonthlyHours - UsedHours);
    public int PreventiveRemaining => Math.Max(0, PreventiveTotal - PreventiveUsed);
    public decimal PercentUsed => MonthlyHours <= 0 ? 0 : 100m * UsedHours / MonthlyHours;
    public bool IsExpired => RenewsAt <= DateTime.UtcNow;

    /// <summary>
    ///     Starts a new billing period with the given plan. Previous consumptions are kept for audit.
    /// </summary>
    public Subscription Renew(Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ApplyPlan(plan);
        return this;
    }

    /// <summary>
    ///     Consumes support hours for a service request in the current period.
    /// </summary>
    /// <returns>False when the hours of this request were already registered in this period.</returns>
    public bool Consume(int serviceRequestId, decimal hours)
    {
        if (serviceRequestId <= 0)
            throw new ArgumentException("A valid service request is required.", nameof(serviceRequestId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hours);
        if (_consumptions.Any(c => c.ServiceRequestId == serviceRequestId && c.PeriodStartedAt == StartedAt))
            return false;
        if (IsExpired)
            throw new InvalidOperationException("Subscription period has expired. Renew before consuming coverage.");
        if (hours > AvailableHours)
            throw new InvalidOperationException("Insufficient support hours in the current subscription period.");

        _consumptions.Add(new CoverageConsumption(serviceRequestId, StartedAt, hours));
        UsedHours += hours;
        Version = Guid.NewGuid();
        return true;
    }

    /// <summary>
    ///     Books one preventive visit of the current period.
    /// </summary>
    public Subscription BookPreventive()
    {
        if (IsExpired)
            throw new InvalidOperationException(
                "Subscription period has expired. Renew before booking preventive support.");
        if (PreventiveRemaining < 1)
            throw new InvalidOperationException("No preventive visits left in this period.");
        PreventiveUsed++;
        Version = Guid.NewGuid();
        return this;
    }

    private void ApplyPlan(Plan plan)
    {
        PlanId = plan.Id;
        PlanName = plan.Name;
        MonthlyHours = plan.Hours;
        PreventiveTotal = plan.PreventiveVisits;
        PreventiveUsed = 0;
        UsedHours = 0;
        StartedAt = DateTime.UtcNow;
        RenewsAt = StartedAt.AddMonths(1);
        Status = "active";
        Version = Guid.NewGuid();
    }
}
