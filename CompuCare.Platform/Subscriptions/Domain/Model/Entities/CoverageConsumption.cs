namespace CompuCare.Platform.Subscriptions.Domain.Model.Entities;

/// <summary>
///     The support hours consumed by one service request in one billing period.
/// </summary>
public class CoverageConsumption
{
    public CoverageConsumption()
    {
    }

    public CoverageConsumption(int serviceRequestId, DateTime periodStartedAt, decimal hours)
    {
        ServiceRequestId = serviceRequestId;
        PeriodStartedAt = periodStartedAt;
        Hours = hours;
    }

    public int Id { get; }
    public int ServiceRequestId { get; private set; }
    public DateTime PeriodStartedAt { get; private set; }
    public decimal Hours { get; private set; }
}
