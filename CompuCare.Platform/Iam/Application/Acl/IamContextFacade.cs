using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.Iam.Application.Acl;

/// <summary>
///     Facade for the IAM context.
/// </summary>
public class IamContextFacade(IUserRepository userRepository) : IIamContextFacade
{
    /// <inheritdoc />
    public async Task<bool> ExistsActiveEmployeeInCompany(int userId, int companyId,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsNoTrackingAsync(userId, cancellationToken);
        return user is { Active: true, Role: UserRoles.Employee } && user.CompanyId == companyId;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsActiveTechnician(int userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsNoTrackingAsync(userId, cancellationToken);
        return user is { Active: true, Role: UserRoles.Technician };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<int>> FetchActiveCompanyManagerIds(int companyId,
        CancellationToken cancellationToken)
    {
        var managers = await userRepository.FindActiveCompanyManagersAsync(companyId, cancellationToken);
        return managers.Select(user => user.Id.Value).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<int, string>> FetchUserNamesByIds(IEnumerable<int> userIds,
        CancellationToken cancellationToken)
    {
        var users = await userRepository.FindByIdsAsync(userIds, cancellationToken);
        return users.ToDictionary(user => user.Id.Value, user => user.Name);
    }
}
