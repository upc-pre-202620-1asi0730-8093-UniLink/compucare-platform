namespace CompuCare.Platform.Iam.Interfaces.Acl;

/// <summary>
///     Facade for the IAM context.
/// </summary>
public interface IIamContextFacade
{
    /// <summary>
    ///     Whether the user is an active employee of the given company.
    /// </summary>
    Task<bool> ExistsActiveEmployeeInCompany(int userId, int companyId, CancellationToken cancellationToken);

    /// <summary>
    ///     Whether the user is an active UniLink technician.
    /// </summary>
    Task<bool> ExistsActiveTechnician(int userId, CancellationToken cancellationToken);

    /// <summary>
    ///     The ids of the active administrators and managers of the given company.
    /// </summary>
    Task<IEnumerable<int>> FetchActiveCompanyManagerIds(int companyId, CancellationToken cancellationToken);

    /// <summary>
    ///     The display names of the given users, keyed by user id.
    /// </summary>
    Task<IReadOnlyDictionary<int, string>> FetchUserNamesByIds(IEnumerable<int> userIds,
        CancellationToken cancellationToken);
}
