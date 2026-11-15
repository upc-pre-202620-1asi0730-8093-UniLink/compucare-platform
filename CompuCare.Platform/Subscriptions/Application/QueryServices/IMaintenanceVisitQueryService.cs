using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;

namespace CompuCare.Platform.Subscriptions.Application.QueryServices;

/// <summary>
///     The maintenance visit query service.
/// </summary>
public interface IMaintenanceVisitQueryService
{
    Task<IEnumerable<MaintenanceVisit>> Handle(GetMaintenanceVisitsByCompanyIdQuery query,
        CancellationToken cancellationToken);
}
