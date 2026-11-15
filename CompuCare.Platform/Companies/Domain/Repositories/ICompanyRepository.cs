using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Domain.Repositories;

/// <summary>
///     The company repository.
/// </summary>
public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken);
}
