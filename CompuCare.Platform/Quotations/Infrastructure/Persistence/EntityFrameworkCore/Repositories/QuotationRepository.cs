using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Quotations.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The quotation repository.
/// </summary>
/// <param name="context">The database context.</param>
public class QuotationRepository(AppDbContext context) : BaseRepository<Quotation>(context), IQuotationRepository
{
    public async Task<IEnumerable<Quotation>> FindAllAsync(CancellationToken cancellationToken)
    {
        return await Context.Set<Quotation>()
            .AsNoTracking()
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quotation>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await Context.Set<Quotation>()
            .AsNoTracking()
            .Where(q => q.CompanyId == companyId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quotation>> FindByTechnicianIdAsync(int technicianId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Quotation>()
            .AsNoTracking()
            .Where(q => q.TechnicianId == technicianId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quotation>> FindByServiceRequestIdAsync(int serviceRequestId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Quotation>()
            .AsNoTracking()
            .Where(q => q.ServiceRequestId == serviceRequestId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
