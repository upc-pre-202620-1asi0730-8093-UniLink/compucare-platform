using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Subscriptions.Application.CommandServices;

/// <summary>
///     The maintenance visit command service.
/// </summary>
public interface IMaintenanceVisitCommandService
{
    Task<Result<MaintenanceVisit>> Handle(ScheduleMaintenanceVisitCommand command,
        CancellationToken cancellationToken);
}
