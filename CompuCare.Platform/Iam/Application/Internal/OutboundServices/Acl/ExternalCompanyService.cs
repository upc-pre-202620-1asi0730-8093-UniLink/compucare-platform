using CompuCare.Platform.Companies.Interfaces.Acl;

namespace CompuCare.Platform.Iam.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the IAM bounded context to interact with Companies capabilities.
/// </summary>
/// <param name="companiesContextFacade">Companies bounded-context facade.</param>
public class ExternalCompanyService(ICompaniesContextFacade companiesContextFacade)
{
    public async Task<bool> ExistsCompanyByTaxId(string taxId, CancellationToken cancellationToken)
    {
        return await companiesContextFacade.ExistsCompanyByTaxId(taxId, cancellationToken);
    }

    /// <returns>The id of the created company, or null when it could not be created.</returns>
    public async Task<int?> CreateCompany(string name, string taxId, CancellationToken cancellationToken)
    {
        var companyId = await companiesContextFacade.CreateCompany(name, taxId, cancellationToken);
        return companyId == 0 ? null : companyId;
    }
}
