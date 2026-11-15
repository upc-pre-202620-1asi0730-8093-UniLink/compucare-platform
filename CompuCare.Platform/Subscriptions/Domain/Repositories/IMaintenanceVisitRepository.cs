using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Domain.Repositories;

/// <summary>
///     The maintenance visit repository.
/// </summary>
public interface IMaintenanceVisitRepository : IBaseRepository<MaintenanceVisit>
{
    Task<IEnumerable<MaintenanceVisit>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
}
