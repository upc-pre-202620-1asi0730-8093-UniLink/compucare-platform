using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The company repository.
/// </summary>
/// <param name="context">The database context.</param>
public class CompanyRepository(AppDbContext context) : BaseRepository<Company>(context), ICompanyRepository
{
    public async Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken)
    {
        return await Context.Set<Company>().AnyAsync(c => c.TaxId == taxId, cancellationToken);
    }
}
