using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Subscriptions.Interfaces.Acl;

namespace CompuCare.Platform.Subscriptions.Application.Acl;

/// <summary>
///     Facade for the Subscriptions context.
/// </summary>
public class SubscriptionsContextFacade(
    ISubscriptionCommandService subscriptionCommandService,
    IPlanRepository planRepository,
    ISubscriptionRepository subscriptionRepository,
    IMaintenanceVisitRepository maintenanceVisitRepository)
    : ISubscriptionsContextFacade
{
    /// <inheritdoc />
    public async Task<bool> ExistsPlanById(int planId, CancellationToken cancellationToken)
    {
        return await planRepository.FindByIdAsNoTrackingAsync(planId, cancellationToken) is not null;
    }

    /// <inheritdoc />
    public async Task<int> ActivateSubscription(int companyId, int planId, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new ActivateSubscriptionCommand(companyId, planId),
            cancellationToken);
        return result.Value?.Id.Value ?? 0;
    }

    /// <inheritdoc />
    public async Task<SupportHoursConsumption> RecordSupportHoursConsumption(int companyId, int serviceRequestId,
        decimal hours, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        if (subscription is null)
            return new SupportHoursConsumption(ESupportHoursConsumptionOutcome.NoSubscription, 0, 0);

        var percentBefore = subscription.PercentUsed;
        try
        {
            var recorded = subscription.Consume(serviceRequestId, hours);
            return new SupportHoursConsumption(
                recorded ? ESupportHoursConsumptionOutcome.Recorded : ESupportHoursConsumptionOutcome.AlreadyRecorded,
                percentBefore, subscription.PercentUsed);
        }
        catch (ArgumentException)
        {
            return new SupportHoursConsumption(ESupportHoursConsumptionOutcome.Rejected, percentBefore, percentBefore);
        }
        catch (InvalidOperationException)
        {
            return new SupportHoursConsumption(
                subscription.IsExpired
                    ? ESupportHoursConsumptionOutcome.Expired
                    : ESupportHoursConsumptionOutcome.InsufficientHours,
                percentBefore, percentBefore);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceVisitSummary>> FetchMaintenanceVisitSummariesByEquipmentId(
        int equipmentId, int companyId, CancellationToken cancellationToken)
    {
        var visits = await maintenanceVisitRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        return visits.Where(visit => visit.Includes(equipmentId))
            .Select(visit => new MaintenanceVisitSummary(visit.Id, visit.Date, visit.Time, visit.Status))
            .ToList();
    }
}
