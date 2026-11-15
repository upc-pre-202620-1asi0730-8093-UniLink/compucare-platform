using CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;
using CompuCare.Platform.ServiceRequests.Domain.Repositories;
using CompuCare.Platform.ServiceRequests.Interfaces.Acl;

namespace CompuCare.Platform.ServiceRequests.Application.Acl;

/// <summary>
///     Facade for the Service Requests context.
/// </summary>
public class ServiceRequestsContextFacade(IServiceRequestRepository serviceRequestRepository)
    : IServiceRequestsContextFacade
{
    /// <inheritdoc />
    public async Task<IEnumerable<int>> FetchEquipmentIdsWithOpenServiceRequests(int companyId,
        CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        return serviceRequests.Where(r => r.IsOpen).Select(r => r.EquipmentId).Distinct().ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ServiceRequestSummary>> FetchServiceRequestSummariesByEquipmentId(int equipmentId,
        int companyId, CancellationToken cancellationToken)
    {
        var serviceRequests = await serviceRequestRepository.FindByEquipmentIdAsync(equipmentId, companyId,
            cancellationToken);
        return serviceRequests.Select(r => new ServiceRequestSummary(r.Id, r.Category, r.Description,
            ServiceRequestStatusLabels.ToLabel(r.Status))).ToList();
    }

    /// <inheritdoc />
    public async Task<QuotableServiceRequest?> FetchQuotableServiceRequest(int serviceRequestId, int technicianId,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsNoTrackingAsync(serviceRequestId,
            cancellationToken);
        if (serviceRequest is null || serviceRequest.TechnicianId != technicianId)
            return null;
        return new QuotableServiceRequest(serviceRequest.Id, serviceRequest.CompanyId,
            serviceRequest.AcceptsQuotations);
    }

    /// <inheritdoc />
    public async Task<bool> SetServiceRequestPaused(int serviceRequestId, int companyId, bool paused,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await serviceRequestRepository.FindByIdAsync(serviceRequestId, cancellationToken);
        if (serviceRequest is null || serviceRequest.CompanyId != companyId)
            return false;
        try
        {
            serviceRequest.SetPaused(paused);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
