using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Domain.Model.Queries;

namespace CompuCare.Platform.Companies.Application.QueryServices;

/// <summary>
///     The equipment query service.
/// </summary>
public interface IEquipmentQueryService
{
    Task<IEnumerable<EquipmentWithStatus>> Handle(GetEquipmentsByCompanyIdQuery query,
        CancellationToken cancellationToken);

    Task<EquipmentHistory?> Handle(GetEquipmentHistoryQuery query, CancellationToken cancellationToken);
}
