using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.ServiceRequests.Domain.Repositories;

/// <summary>
///     The service request repository.
/// </summary>
public interface IServiceRequestRepository : IBaseRepository<ServiceRequest>
{
    Task<IEnumerable<ServiceRequest>> FindAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequest>> FindByCompanyIdAsync(int companyId, CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequest>> FindByRequesterIdAsync(int requesterId, CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequest>> FindByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken);

    Task<IEnumerable<ServiceRequest>> FindByEquipmentIdAsync(int equipmentId, int companyId,
        CancellationToken cancellationToken);
}
