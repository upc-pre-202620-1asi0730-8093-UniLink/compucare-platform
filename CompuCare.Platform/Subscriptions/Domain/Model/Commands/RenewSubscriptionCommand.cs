namespace CompuCare.Platform.Subscriptions.Domain.Model.Commands;

/// <summary>
///     Starts a new billing period of the current plan of a company.
/// </summary>
public record RenewSubscriptionCommand(int CompanyId);
