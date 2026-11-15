using CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.QueryServices;

/// <summary>
///     Completes service requests with the equipment and user names owned by other bounded contexts.
/// </summary>
public class ServiceRequestDetailsBuilder(
    ExternalEquipmentService externalEquipmentService,
    ExternalUserService externalUserService)
{
    public async Task<IReadOnlyList<ServiceRequestDetails>> Build(IEnumerable<ServiceRequest> serviceRequests,
        CancellationToken cancellationToken)
    {
        var requests = serviceRequests.ToList();
        if (requests.Count == 0) return [];

        var equipmentNames = await externalEquipmentService.FetchEquipmentNamesByIds(
            requests.Select(r => r.EquipmentId), cancellationToken);
        var userIds = requests.Select(r => r.RequesterId)
            .Concat(requests.Where(r => r.TechnicianId.HasValue).Select(r => r.TechnicianId!.Value));
        var userNames = await externalUserService.FetchUserNamesByIds(userIds, cancellationToken);

        return requests.Select(r => new ServiceRequestDetails(
            r,
            equipmentNames.GetValueOrDefault(r.EquipmentId),
            userNames.GetValueOrDefault(r.RequesterId),
            r.TechnicianId.HasValue ? userNames.GetValueOrDefault(r.TechnicianId.Value) : null)).ToList();
    }

    public async Task<ServiceRequestDetails> Build(ServiceRequest serviceRequest, CancellationToken cancellationToken)
    {
        var details = await Build([serviceRequest], cancellationToken);
        return details[0];
    }
}
