using CompuCare.Platform.Subscriptions.Domain.Model.Commands;
using CompuCare.Platform.Subscriptions.Interfaces.Rest.Resources;

namespace CompuCare.Platform.Subscriptions.Interfaces.Rest.Transform;

public static class SubscribeCommandFromResourceAssembler
{
    public static SubscribeCommand ToCommandFromResource(SubscribeResource resource, int companyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SubscribeCommand(companyId, resource.PlanId, resource.Billing?.Name ?? string.Empty,
            resource.Billing?.Email ?? string.Empty);
    }
}
