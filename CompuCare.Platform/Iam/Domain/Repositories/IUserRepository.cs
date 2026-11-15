using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Iam.Domain.Repositories;

/// <summary>
///     The user repository.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<IEnumerable<User>> FindByCompanyIdAndRoleAsync(int companyId, string role, CancellationToken cancellationToken);

    Task<IEnumerable<User>> FindActiveByRoleAsync(string role, CancellationToken cancellationToken);

    Task<IEnumerable<User>> FindActiveCompanyManagersAsync(int companyId, CancellationToken cancellationToken);

    Task<IEnumerable<User>> FindByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}
