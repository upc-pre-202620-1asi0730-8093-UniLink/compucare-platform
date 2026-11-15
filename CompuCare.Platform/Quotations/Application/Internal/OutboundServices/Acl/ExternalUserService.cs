using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.Quotations.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Quotations bounded context to interact with IAM capabilities.
/// </summary>
/// <param name="iamContextFacade">IAM bounded-context facade.</param>
public class ExternalUserService(IIamContextFacade iamContextFacade)
{
    public async Task<IEnumerable<int>> FetchActiveCompanyManagerIds(int companyId,
        CancellationToken cancellationToken)
    {
        return await iamContextFacade.FetchActiveCompanyManagerIds(companyId, cancellationToken);
    }
}
