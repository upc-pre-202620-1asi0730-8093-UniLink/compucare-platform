using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Companies.Application.CommandServices;

/// <summary>
///     The equipment command service.
/// </summary>
public interface IEquipmentCommandService
{
    Task<Result<EquipmentWithStatus>> Handle(CreateEquipmentCommand command, CancellationToken cancellationToken);

    Task<Result<EquipmentWithStatus>> Handle(AssignEquipmentCommand command, CancellationToken cancellationToken);
}
