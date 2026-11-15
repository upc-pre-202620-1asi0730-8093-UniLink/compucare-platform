namespace CompuCare.Platform.Subscriptions.Domain.Model.Commands;

/// <summary>
///     Activates the first subscription of a company with the given plan.
/// </summary>
public record ActivateSubscriptionCommand(int CompanyId, int PlanId);
