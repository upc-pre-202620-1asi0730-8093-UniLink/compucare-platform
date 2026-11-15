using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Application.QueryServices;
using CompuCare.Platform.ServiceRequests.Domain.Model.Queries;
using CompuCare.Platform.ServiceRequests.Domain.Repositories;

namespace CompuCare.Platform.ServiceRequests.Application.Internal.QueryServices;

public class ServiceRequestQueryService(
    IServiceRequestRepository serviceRequestRepository,
    ServiceRequestDetailsBuilder serviceRequestDetailsBuilder)
    : IServiceRequestQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<ServiceRequestDetails>> Handle(GetAllServiceRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindAllAsync(cancellationToken);
        return await serviceRequestDetailsBuilder.Build(serviceRequests, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindByCompanyIdAsync(query.CompanyId, cancellationToken);
        return await serviceRequestDetailsBuilder.Build(serviceRequests, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByRequesterIdQuery query,
        CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindByRequesterIdAsync(query.RequesterId,
            cancellationToken);
        return await serviceRequestDetailsBuilder.Build(serviceRequests, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ServiceRequestDetails>> Handle(GetServiceRequestsByTechnicianIdQuery query,
        CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindByTechnicianIdAsync(query.TechnicianId,
            cancellationToken);
        return await serviceRequestDetailsBuilder.Build(serviceRequests, cancellationToken);
    }
}
