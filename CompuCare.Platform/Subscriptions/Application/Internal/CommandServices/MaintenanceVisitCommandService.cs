using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.Subscriptions.Domain.Model;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Domain.Model.Errors;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Application.Internal.CommandServices;

public class MaintenanceVisitCommandService(
    ISubscriptionRepository subscriptionRepository,
    IMaintenanceVisitRepository maintenanceVisitRepository,
    ExternalEquipmentService externalEquipmentService,
    IUnitOfWork unitOfWork)
    : IMaintenanceVisitCommandService
{
    /// <summary>
    ///     Peru does not observe daylight saving time, so its offset from UTC is constant.
    /// </summary>
    private static readonly TimeSpan PeruUtcOffset = TimeSpan.FromHours(-5);

    /// <inheritdoc />
    public async Task<Result<MaintenanceVisit>> Handle(ScheduleMaintenanceVisitCommand command,
        CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParseExact(command.Date, "yyyy-MM-dd", out var date))
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.InvalidMaintenanceData,
                SubscriptionsError.InvalidMaintenanceData);

        var todayInPeru = DateOnly.FromDateTime(DateTime.UtcNow.Add(PeruUtcOffset));
        if (date < todayInPeru)
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.MaintenanceDateInPast,
                SubscriptionsError.MaintenanceDateInPast);

        var subscription = await subscriptionRepository.FindByCompanyIdAsync(command.CompanyId, cancellationToken);
        if (subscription is null)
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.SubscriptionNotFound,
                SubscriptionsError.SubscriptionNotFound);

        MaintenanceVisit visit;
        try
        {
            visit = new MaintenanceVisit(command, subscription.Id);
        }
        catch (ArgumentException)
        {
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.InvalidMaintenanceData,
                SubscriptionsError.InvalidMaintenanceData);
        }

        if (!await externalEquipmentService.AllEquipmentsBelongToCompany(visit.EquipmentIds, command.CompanyId,
                cancellationToken))
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.MaintenanceEquipmentNotInCompany,
                SubscriptionsError.MaintenanceEquipmentNotInCompany);

        if (subscription.IsExpired)
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.SubscriptionExpired,
                SubscriptionsError.SubscriptionExpired);
        if (subscription.PreventiveRemaining < 1)
            return Result<MaintenanceVisit>.Failure(SubscriptionsErrors.NoPreventiveVisitsLeft,
                SubscriptionsError.NoPreventiveVisitsLeft);

        subscription.BookPreventive();
        await maintenanceVisitRepository.AddAsync(visit, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<MaintenanceVisit>.Success(visit);
    }
}
