using CompuCare.Platform.ServiceRequests.Application.Model;
using CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;
using CompuCare.Platform.ServiceRequests.Interfaces.Rest.Resources;

namespace CompuCare.Platform.ServiceRequests.Interfaces.Rest.Transform;

public static class ServiceRequestResourceFromEntityAssembler
{
    public static ServiceRequestResource ToResourceFromEntity(ServiceRequestDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);
        var request = details.ServiceRequest;
        return new ServiceRequestResource(request.Id, request.CompanyId, request.RequesterId, request.EquipmentId,
            request.TechnicianId, request.Category, request.Description,
            ServiceRequestStatusLabels.ToLabel(request.Status), request.Paused, request.CreatedAt,
            details.EquipmentName, details.EmployeeName, details.TechnicianName, request.Diagnosis, request.WorkDone,
            request.WorkHours,
            request.Timeline.OrderBy(p => p.At)
                .Select(p => new ServiceRequestTimelineResource(ServiceRequestStatusLabels.ToLabel(p.Status), p.At))
                .ToList());
    }
}
