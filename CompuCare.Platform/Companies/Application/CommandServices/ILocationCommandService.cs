using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Companies.Application.CommandServices;

/// <summary>
///     The location command service.
/// </summary>
public interface ILocationCommandService
{
    Task<Result<Location>> Handle(CreateLocationCommand command, CancellationToken cancellationToken);

    Task<Result<Location>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken);
}
