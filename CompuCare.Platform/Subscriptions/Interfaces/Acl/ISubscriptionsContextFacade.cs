namespace CompuCare.Platform.Subscriptions.Interfaces.Acl;

/// <summary>
///     Facade for the Subscriptions context.
/// </summary>
public interface ISubscriptionsContextFacade
{
    /// <summary>
    ///     Whether a plan with the given id is published.
    /// </summary>
    Task<bool> ExistsPlanById(int planId, CancellationToken cancellationToken);

    /// <summary>
    ///     Activates (or switches) the subscription of a company to the given plan.
    /// </summary>
    /// <returns>The id of the subscription if successful, 0 otherwise.</returns>
    Task<int> ActivateSubscription(int companyId, int planId, CancellationToken cancellationToken);

    /// <summary>
    ///     Consumes support hours of the company subscription for a service request. The change is tracked
    ///     but not saved: the calling command commits it together with its own changes.
    /// </summary>
    Task<SupportHoursConsumption> RecordSupportHoursConsumption(int companyId, int serviceRequestId, decimal hours,
        CancellationToken cancellationToken);

    /// <summary>
    ///     The preventive maintenance visits of the company that include the equipment.
    /// </summary>
    Task<IEnumerable<MaintenanceVisitSummary>> FetchMaintenanceVisitSummariesByEquipmentId(int equipmentId,
        int companyId, CancellationToken cancellationToken);
}
