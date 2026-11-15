using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model.Queries;

namespace CompuCare.Platform.ServiceRequests.Application.QueryServices;

/// <summary>
///     The service request query service.
/// </summary>
public interface IServiceRequestQueryService
{
    Task<IEnumerable<ServiceRequestDetails>> Handle(GetAllServiceRequestsQuery query,
        CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByCompanyIdQuery query,
        CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByRequesterIdQuery query,
        CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByTechnicianIdQuery query,
        CancellationToken cancellationToken);
}
