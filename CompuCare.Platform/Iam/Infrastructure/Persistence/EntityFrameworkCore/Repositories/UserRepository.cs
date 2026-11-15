using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The user repository.
/// </summary>
/// <param name="context">The database context.</param>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> FindByCompanyIdAndRoleAsync(int companyId, string role,
        CancellationToken cancellationToken)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId && user.Role == role)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> FindActiveByRoleAsync(string role, CancellationToken cancellationToken)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .Where(user => user.Role == role && user.Active)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> FindActiveCompanyManagersAsync(int companyId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId && user.Active &&
                           (user.Role == UserRoles.Administrator || user.Role == UserRoles.CompanyManager))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> FindByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var userIds = ids.Distinct().Select(id => new UserId(id)).ToList();
        if (userIds.Count == 0) return [];
        return await Context.Set<User>()
            .AsNoTracking()
            .Where(user => userIds.Contains(user.Id))
            .ToListAsync(cancellationToken);
    }
}
