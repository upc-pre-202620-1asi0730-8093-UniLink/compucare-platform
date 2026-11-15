using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The location repository.
/// </summary>
/// <param name="context">The database context.</param>
public class LocationRepository(AppDbContext context) : BaseRepository<Location>(context), ILocationRepository
{
    public async Task<IEnumerable<Location>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await Context.Set<Location>()
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId)
            .OrderBy(l => l.Name)
            .ThenBy(l => l.Desk)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCompanyIdAndNameAndDeskAsync(int companyId, string name, string desk,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Location>()
            .AnyAsync(l => l.CompanyId == companyId && l.Name == name && l.Desk == desk, cancellationToken);
    }
}
