namespace CompuCare.Platform.Subscriptions.Domain.Model.Commands;

/// <summary>
///     Subscribes a company to a plan, or switches its current subscription to that plan.
/// </summary>
public record SubscribeCommand(int CompanyId, int PlanId, string BillingName, string BillingEmail);
