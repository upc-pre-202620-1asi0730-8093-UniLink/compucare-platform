using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class PlanResourceFromEntityAssembler
{
    public static PlanResource ToResourceFromEntity(Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return new PlanResource(plan.Id, plan.Code, plan.Name, plan.Price, plan.Hours, plan.PreventiveVisits);
    }
}
