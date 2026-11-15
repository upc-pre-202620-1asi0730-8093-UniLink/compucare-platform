using CompuCare.Platform.Iam.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;

/// <summary>
///     ACL service used by the Service Requests bounded context to interact with IAM capabilities.
/// </summary>
/// <param name="iamContextFacade">IAM bounded-context facade.</param>
public class ExternalUserService(IIamContextFacade iamContextFacade)
{
    public async Task<bool> ExistsActiveTechnician(int technicianId, CancellationToken cancellationToken)
    {
        return await iamContextFacade.ExistsActiveTechnician(technicianId, cancellationToken);
    }

    public async Task<IEnumerable<int>> FetchActiveCompanyManagerIds(int companyId,
        CancellationToken cancellationToken)
    {
        return await iamContextFacade.FetchActiveCompanyManagerIds(companyId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, string>> FetchUserNamesByIds(IEnumerable<int> userIds,
        CancellationToken cancellationToken)
    {
        return await iamContextFacade.FetchUserNamesByIds(userIds, cancellationToken);
    }
}
