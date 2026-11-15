using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;
using CompuCare.Platform.Subscriptions.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Application.Internal.QueryServices;

public class MaintenanceVisitQueryService(IMaintenanceVisitRepository maintenanceVisitRepository)
    : IMaintenanceVisitQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceVisit>> Handle(GetMaintenanceVisitsByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await maintenanceVisitRepository.FindByCompanyIdAsync(query.CompanyId, cancellationToken);
    }
}
