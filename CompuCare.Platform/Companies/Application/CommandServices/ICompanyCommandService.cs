using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Shared.Application.Model;

namespace CompuCare.Platform.Companies.Application.CommandServices;

/// <summary>
///     The company command service.
/// </summary>
public interface ICompanyCommandService
{
    Task<Result<Company>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken);
}
