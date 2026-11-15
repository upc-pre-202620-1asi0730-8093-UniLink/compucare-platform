namespace CompuCare.Platform.Subscriptions.Domain.Model;

public enum SubscriptionsError
{
    None,
    PlanNotFound,
    SubscriptionNotFound,
    InvalidBillingData,
    InvalidMaintenanceData,
    MaintenanceDateInPast,
    MaintenanceEquipmentNotInCompany,
    SubscriptionExpired,
    NoPreventiveVisitsLeft,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
