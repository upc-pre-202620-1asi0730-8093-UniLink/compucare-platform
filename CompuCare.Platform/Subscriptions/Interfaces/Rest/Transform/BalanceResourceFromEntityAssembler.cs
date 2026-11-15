using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class BalanceResourceFromEntityAssembler
{
    public static BalanceResource ToResourceFromEntity(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        return new BalanceResource(subscription.Id, subscription.CompanyId, subscription.PlanId,
            subscription.PlanName, subscription.MonthlyHours, subscription.UsedHours, subscription.AvailableHours,
            subscription.PreventiveTotal, subscription.PreventiveUsed, subscription.PreventiveRemaining,
            Math.Round(subscription.PercentUsed), subscription.StartedAt, subscription.RenewsAt,
            subscription.Status);
    }
}
