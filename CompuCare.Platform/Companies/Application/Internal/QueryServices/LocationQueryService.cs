using CompuCare.Platform.Companies.Application.QueryServices;
using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Queries;
using CompuCare.Platform.Companies.Domain.Repositories;

namespace CompuCare.Platform.Companies.Application.Internal.QueryServices;

public class LocationQueryService(ILocationRepository locationRepository) : ILocationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Location>> Handle(GetLocationsByCompanyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await locationRepository.FindByCompanyIdAsync(query.CompanyId, cancellationToken);
    }
}
