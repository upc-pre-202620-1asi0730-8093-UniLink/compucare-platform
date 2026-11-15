using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The equipment repository.
/// </summary>
/// <param name="context">The database context.</param>
public class EquipmentRepository(AppDbContext context) : BaseRepository<Equipment>(context), IEquipmentRepository
{
    public async Task<IEnumerable<Equipment>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await Context.Set<Equipment>()
            .AsNoTracking()
            .Where(e => e.CompanyId == companyId)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCompanyIdAndSerialNumberAsync(int companyId, string serialNumber,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Equipment>()
            .AnyAsync(e => e.CompanyId == companyId && e.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> FindByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var equipmentIds = ids.Distinct().Select(id => new EquipmentId(id)).ToList();
        if (equipmentIds.Count == 0) return [];
        return await Context.Set<Equipment>()
            .AsNoTracking()
            .Where(e => equipmentIds.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }
}
