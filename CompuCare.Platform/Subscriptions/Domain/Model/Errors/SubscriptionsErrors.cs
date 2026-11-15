using CompuCare.Platform.Shared.Domain.Model.Errors;

namespace CompuCare.Platform.Subscriptions.Domain.Model.Errors;

public static class SubscriptionsErrors
{
    public static readonly Error PlanNotFound =
        Error.NotFound("Subscriptions.PlanNotFound", "The selected plan was not found.");

    public static readonly Error SubscriptionNotFound =
        Error.NotFound("Subscriptions.SubscriptionNotFound", "The company has no active subscription.");

    public static readonly Error InvalidBillingData =
        Error.Validation("Subscriptions.InvalidBillingData", "Valid billing details are required.");

    public static readonly Error InvalidMaintenanceData =
        Error.Validation("Subscriptions.InvalidMaintenanceData",
            "A valid date, time and non-duplicated list of devices are required.");

    public static readonly Error MaintenanceDateInPast =
        Error.Validation("Subscriptions.MaintenanceDateInPast", "The maintenance date cannot be in the past.");

    public static readonly Error MaintenanceEquipmentNotInCompany =
        Error.Validation("Subscriptions.MaintenanceEquipmentNotInCompany",
            "All selected devices must belong to your company.");

    public static readonly Error SubscriptionExpired =
        Error.Conflict("Subscriptions.SubscriptionExpired", "The subscription period has expired. Renew it first.");

    public static readonly Error NoPreventiveVisitsLeft =
        Error.Conflict("Subscriptions.NoPreventiveVisitsLeft", "No preventive visits left in this period.");
}
