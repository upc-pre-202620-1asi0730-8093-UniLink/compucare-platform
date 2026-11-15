using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;
using CompuCare.Platform.ServiceRequests.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.ServiceRequests.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The service request repository.
/// </summary>
/// <param name="context">The database context.</param>
public class ServiceRequestRepository(AppDbContext context)
    : BaseRepository<ServiceRequest>(context), IServiceRequestRepository
{
    public async Task<IEnumerable<ServiceRequest>> FindAllAsync(CancellationToken cancellationToken)
    {
        return await Context.Set<ServiceRequest>()
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> FindByCompanyIdAsync(int companyId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ServiceRequest>()
            .AsNoTracking()
            .Where(r => r.CompanyId == companyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> FindByRequesterIdAsync(int requesterId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ServiceRequest>()
            .AsNoTracking()
            .Where(r => r.RequesterId == requesterId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> FindByTechnicianIdAsync(int technicianId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ServiceRequest>()
            .AsNoTracking()
            .Where(r => r.TechnicianId == technicianId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceRequest>> FindByEquipmentIdAsync(int equipmentId, int companyId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ServiceRequest>()
            .AsNoTracking()
            .Where(r => r.EquipmentId == equipmentId && r.CompanyId == companyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
