using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;

namespace CompuCare.Platform.Subscriptions.Application.QueryServices;

/// <summary>
///     The plan query service.
/// </summary>
public interface IPlanQueryService
{
    Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken);
}
