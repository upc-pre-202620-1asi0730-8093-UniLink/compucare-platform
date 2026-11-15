namespace CompuCare.Platform.Subscriptions.Interfaces.Acl;

/// <summary>
///     The outcome of consuming support hours for a service request.
/// </summary>
public enum ESupportHoursConsumptionOutcome
{
    Recorded,
    AlreadyRecorded,
    NoSubscription,
    Expired,
    InsufficientHours,
    Rejected
}

/// <summary>
///     Published language of the Subscriptions context for a support hours consumption.
/// </summary>
/// <param name="Outcome">What happened to the request.</param>
/// <param name="PercentUsedBefore">Percentage of the monthly hours used before the consumption.</param>
/// <param name="PercentUsedAfter">Percentage of the monthly hours used after the consumption.</param>
public record SupportHoursConsumption(
    ESupportHoursConsumptionOutcome Outcome,
    decimal PercentUsedBefore,
    decimal PercentUsedAfter);
