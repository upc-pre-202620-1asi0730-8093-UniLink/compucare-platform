using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Queries;

namespace CompuCare.Platform.Companies.Application.QueryServices;

/// <summary>
///     The location query service.
/// </summary>
public interface ILocationQueryService
{
    Task<IEnumerable<Location>> Handle(GetLocationsByCompanyIdQuery query, CancellationToken cancellationToken);
}
