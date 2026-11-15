using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Companies.Interfaces.Acl;

namespace CompuCare.Platform.Companies.Application.Acl;

/// <summary>
///     Facade for the Companies context.
/// </summary>
public class CompaniesContextFacade(
    ICompanyCommandService companyCommandService,
    ICompanyRepository companyRepository,
    IEquipmentRepository equipmentRepository)
    : ICompaniesContextFacade
{
    /// <inheritdoc />
    public async Task<bool> ExistsCompanyByTaxId(string taxId, CancellationToken cancellationToken)
    {
        return await companyRepository.ExistsByTaxIdAsync(taxId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CreateCompany(string name, string taxId, CancellationToken cancellationToken)
    {
        var result = await companyCommandService.Handle(new CreateCompanyCommand(name, taxId), cancellationToken);
        return result.Value?.Id.Value ?? 0;
    }

    /// <inheritdoc />
    public async Task<bool> IsEquipmentAssignedToEmployee(int equipmentId, int companyId, int employeeId,
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.FindByIdAsNoTrackingAsync(equipmentId, cancellationToken);
        return equipment is not null && equipment.CompanyId == companyId && equipment.IsAssignedTo(employeeId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<int, string>> FetchEquipmentNamesByIds(IEnumerable<int> equipmentIds,
        CancellationToken cancellationToken)
    {
        var equipments = await equipmentRepository.FindByIdsAsync(equipmentIds, cancellationToken);
        return equipments.ToDictionary(e => e.Id.Value, e => e.Name);
    }

    /// <inheritdoc />
    public async Task<bool> AllEquipmentsBelongToCompany(IEnumerable<int> equipmentIds, int companyId,
        CancellationToken cancellationToken)
    {
        var requested = equipmentIds.ToList();
        var equipments = await equipmentRepository.FindByIdsAsync(requested, cancellationToken);
        var owned = equipments.Where(e => e.CompanyId == companyId).Select(e => e.Id.Value).ToHashSet();
        return requested.Count > 0 && requested.All(owned.Contains);
    }
}
