using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Companies bounded context to interact with IAM capabilities.
/// </summary>
/// <param name="iamContextFacade">IAM bounded-context facade.</param>
public class ExternalEmployeeService(IIamContextFacade iamContextFacade)
{
    public async Task<bool> ExistsActiveEmployeeInCompany(int employeeId, int companyId,
        CancellationToken cancellationToken)
    {
        return await iamContextFacade.ExistsActiveEmployeeInCompany(employeeId, companyId, cancellationToken);
    }
}
