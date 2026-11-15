using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The maintenance visit repository.
/// </summary>
/// <param name="context">The database context.</param>
public class MaintenanceVisitRepository(AppDbContext context)
    : BaseRepository<MaintenanceVisit>(context), IMaintenanceVisitRepository
{
    public async Task<IEnumerable<MaintenanceVisit>> FindByCompanyIdAsync(int companyId,
        CancellationToken cancellationToken)
    {
        var visits = await Context.Set<MaintenanceVisit>()
            .AsNoTracking()
            .Where(v => v.CompanyId == companyId)
            .ToListAsync(cancellationToken);
        return visits.OrderByDescending(v => v.Date).ThenBy(v => v.Time).ToList();
    }
}
