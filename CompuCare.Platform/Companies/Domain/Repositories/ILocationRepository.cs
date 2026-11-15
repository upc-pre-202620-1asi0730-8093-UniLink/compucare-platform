using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Domain.Repositories;

/// <summary>
///     The location repository.
/// </summary>
public interface ILocationRepository : IBaseRepository<Location>
{
    Task<IEnumerable<Location>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);

    Task<bool> ExistsByCompanyIdAndNameAndDeskAsync(int companyId, string name, string desk,
        CancellationToken cancellationToken);
}
