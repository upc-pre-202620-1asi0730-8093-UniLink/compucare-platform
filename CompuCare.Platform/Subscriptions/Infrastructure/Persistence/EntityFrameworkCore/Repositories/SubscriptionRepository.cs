using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The subscription repository.
/// </summary>
/// <param name="context">The database context.</param>
public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<Subscription?> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.CompanyId == companyId, cancellationToken);
    }

    public async Task<Subscription?> FindByCompanyIdAsNoTrackingAsync(int companyId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Subscription>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CompanyId == companyId, cancellationToken);
    }
}
