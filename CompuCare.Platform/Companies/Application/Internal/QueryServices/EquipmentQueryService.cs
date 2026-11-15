using CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Application.QueryServices;
using CompuCare.Platform.Companies.Domain.Model.Queries;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Companies.Domain.Repositories;

namespace CompuCare.Platform.Companies.Application.Internal.QueryServices;

public class EquipmentQueryService(
    IEquipmentRepository equipmentRepository,
    ExternalServiceRequestService externalServiceRequestService,
    ExternalMaintenanceService externalMaintenanceService)
    : IEquipmentQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<EquipmentWithStatus>> Handle(GetEquipmentsByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        var equipments = await equipmentRepository.FindByCompanyIdAsync(query.CompanyId, cancellationToken);

        if (query.AssignedEmployeeId.HasValue)
            equipments = equipments.Where(e => e.AssignedEmployeeId == query.AssignedEmployeeId);
        if (query.LocationId.HasValue)
            equipments = equipments.Where(e => e.LocationId == query.LocationId);
        if (!string.IsNullOrWhiteSpace(query.Type))
            equipments = equipments.Where(e => e.Type == query.Type);
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            equipments = equipments.Where(e => $"{e.Code} {e.Name} {e.SerialNumber}"
                .Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase));

        var inRepair = await externalServiceRequestService.FetchEquipmentIdsInRepair(query.CompanyId,
            cancellationToken);
        var result = equipments.Select(e => new EquipmentWithStatus(e,
            inRepair.Contains(e.Id) ? EEquipmentStatus.InRepair : EEquipmentStatus.Operational));

        if (query.Status.HasValue)
            result = result.Where(e => e.Status == query.Status);

        return result.ToList();
    }

    /// <inheritdoc />
    public async Task<EquipmentHistory?> Handle(GetEquipmentHistoryQuery query, CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.FindByIdAsNoTrackingAsync(query.EquipmentId, cancellationToken);
        if (equipment is null || equipment.CompanyId != query.CompanyId)
            return null;

        var inRepair = await externalServiceRequestService.FetchEquipmentIdsInRepair(query.CompanyId,
            cancellationToken);
        var serviceRequests = await externalServiceRequestService.FetchServiceRequestsByEquipmentId(
            query.EquipmentId, query.CompanyId, cancellationToken);
        var maintenances = await externalMaintenanceService.FetchMaintenancesByEquipmentId(query.EquipmentId,
            query.CompanyId, cancellationToken);

        var status = inRepair.Contains(equipment.Id) ? EEquipmentStatus.InRepair : EEquipmentStatus.Operational;
        return new EquipmentHistory(new EquipmentWithStatus(equipment, status), serviceRequests, maintenances);
    }
}
