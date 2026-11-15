using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Domain.Repositories;

/// <summary>
///     The equipment repository.
/// </summary>
public interface IEquipmentRepository : IBaseRepository<Equipment>
{
    Task<IEnumerable<Equipment>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);

    Task<bool> ExistsByCompanyIdAndSerialNumberAsync(int companyId, string serialNumber,
        CancellationToken cancellationToken);

    Task<IEnumerable<Equipment>> FindByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}
