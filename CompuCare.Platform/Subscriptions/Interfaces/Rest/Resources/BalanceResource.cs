namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

public record BalanceResource(
    int Id,
    int CompanyId,
    int PlanId,
    string PlanName,
    decimal HoursTotal,
    decimal HoursUsed,
    decimal HoursRemaining,
    int PreventiveTotal,
    int PreventiveUsed,
    int PreventiveRemaining,
    decimal PercentUsed,
    DateTime StartedAt,
    DateTime RenewsAt,
    string Status);
