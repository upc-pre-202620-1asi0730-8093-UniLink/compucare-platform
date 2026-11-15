namespace CompuCare.Platform.Companies.Interfaces.Acl;

/// <summary>
///     Facade for the Companies context.
/// </summary>
public interface ICompaniesContextFacade
{
    /// <summary>
    ///     Whether a company with the given tax identifier (RUC) is already registered.
    /// </summary>
    Task<bool> ExistsCompanyByTaxId(string taxId, CancellationToken cancellationToken);

    /// <summary>
    ///     Registers a company.
    /// </summary>
    /// <returns>The id of the created company if successful, 0 otherwise.</returns>
    Task<int> CreateCompany(string name, string taxId, CancellationToken cancellationToken);

    /// <summary>
    ///     Whether the equipment belongs to the company and is currently assigned to the employee.
    /// </summary>
    Task<bool> IsEquipmentAssignedToEmployee(int equipmentId, int companyId, int employeeId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     The names of the given equipment, keyed by equipment id.
    /// </summary>
    Task<IReadOnlyDictionary<int, string>> FetchEquipmentNamesByIds(IEnumerable<int> equipmentIds,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Whether every given equipment id exists and belongs to the company.
    /// </summary>
    Task<bool> AllEquipmentsBelongToCompany(IEnumerable<int> equipmentIds, int companyId,
        CancellationToken cancellationToken);
}
