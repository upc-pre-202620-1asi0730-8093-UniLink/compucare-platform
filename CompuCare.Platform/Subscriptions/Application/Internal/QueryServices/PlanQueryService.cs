using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.Queries;
using CompuCare.Platform.Subscriptions.Domain.Repositories;

namespace CompuCare.Platform.Subscriptions.Application.Internal.QueryServices;

public class PlanQueryService(IPlanRepository planRepository) : IPlanQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query, CancellationToken cancellationToken)
    {
        var plans = await planRepository.ListAsNoTrackingAsync(cancellationToken);
        return plans.OrderBy(plan => plan.Price).ToList();
    }
}
