namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

public record BillingResource(string Name, string Email);

public record SubscribeResource(int PlanId, BillingResource Billing);
